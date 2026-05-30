# OAuth Integration with Google & GitHub - Implementation Guide

## Overview

This document details the implementation of OAuth 2.0 authentication with Google and GitHub providers. The implementation follows Clean Architecture principles and allows users to sign in using their social accounts while also supporting account linking.

---

## Architecture

### Design Decision: Password on User, Separate AuthProvider Table

```
User                          AuthProvider
────────                      ────────────
Id (PK)                       Id (PK)
Name                          UserId (FK → User)
LastName                      Provider (enum: Google, GitHub)
Email                         ProviderUserId
Password (nullable)           CreatedAt
EmailVerified
ResetPasswordCodes
Roles
AuthProviders (navigation)
```

**Rationale:**
- Password is a core user credential, not an authentication provider
- OAuth providers are *methods* of authentication, not part of user identity
- Easy to query all linked accounts for a user (`user.AuthProviders`)
- Easy to unlink an account (delete row)
- Future auth methods (Magic Links, SMS, WebAuthn) can be added as new providers

---

## Implementation Details

### Phase 1: Domain Layer

#### 1.1 OAuthProviderType Enum
**File:** `Domain/AuthProvider/OAuthProviderType.cs`
```csharp
public enum OAuthProviderType
{
    Google,
    GitHub
}
```

#### 1.2 AuthProvider Entity
**File:** `Domain/AuthProvider/AuthProvider.cs`
```csharp
public class AuthProvider : BaseEntity
{
    public required Guid UserId { get; set; }
    public required OAuthProviderType Provider { get; set; }
    public required string ProviderUserId { get; set; }
    public User.User User { get; set; }
}
```

#### 1.3 User Entity Updates
**File:** `Domain/User/User.cs`
- `Password` made nullable (optional for OAuth-only users)
- Added `List<AuthProvider.AuthProvider> AuthProviders` navigation property

---

### Phase 2: Infrastructure Layer

#### 2.1 EF Configuration
**File:** `Infrastructure/Persistence/EntityFramework/AppDbContext.cs`
- Added `DbSet<AuthProvider> AuthProviders`
- Configured unique constraints:
  - `(Provider, ProviderUserId)` - Each OAuth account is unique
  - `(UserId, Provider)` - One account per provider per user
- Configured cascade delete relationship

#### 2.2 Design-Time DbContext Factory
**File:** `Infrastructure/Persistence/DesignTimeDbContextFactory.cs`
- Required for EF migrations to work without running the application

#### 2.3 Database Migration
```
dotnet ef migrations add AddAuthProvidersTable --startup-project CleanArchTemplate.Presentation.Api --project CleanArchTemplate.Infrastructure
```

---

### Phase 3: Application Layer - Repositories

#### 3.1 IAuthProviderRepository
**File:** `Application/Repositories/AuthProvider/IAuthProviderRepository.cs`
```csharp
public interface IAuthProviderRepository
{
    Task<AuthProvider?> GetByProviderAsync(OAuthProviderType provider, string providerUserId, CancellationToken ct);
    Task<AuthProvider?> GetByUserAndProviderAsync(Guid userId, OAuthProviderType provider, CancellationToken ct);
    Task<Guid> CreateAsync(AuthProvider authProvider, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> DeleteByUserAndProviderAsync(Guid userId, OAuthProviderType provider, CancellationToken ct);
}
```

#### 3.2 AuthProviderRepository Implementation
**File:** `Application/Repositories/AuthProvider/AuthProviderRepository.cs`
- Implements all repository methods using EF Core
- Includes eager loading of User and Roles

#### 3.3 IUserRepository Updates
**File:** `Application/Repositories/User/IUserRepository.cs`
- Added `GetByEmailWithAuthProvidersAsync` method
- Implementation includes `Include(x => x.AuthProviders)`

---

### Phase 4: Application Layer - OAuth Service

#### 4.1 IOAuthService Interface
**File:** `Application/Services/Auth/OAuthService/IOAuthService.cs`
```csharp
public interface IOAuthService
{
    string GetAuthorizationUrl(OAuthProviderType provider);
    Task<OAuthUserInfo> GetUserInfoAsync(OAuthProviderType provider, string code, CancellationToken ct);
}
```

#### 4.2 OAuthUserInfo DTO
**File:** `Application/Services/Auth/OAuthService/OAuthUserInfo.cs`
```csharp
public record OAuthUserInfo(string ProviderId, string Email, string Name, string? LastName);
```

#### 4.3 OAuthOptions Configuration
**File:** `Application/Services/Auth/OAuthService/Options/OAuthOptions.cs`
```csharp
public class OAuthOptions
{
    public GoogleOAuthOptions Google { get; set; }
    public GitHubOAuthOptions GitHub { get; set; }
}
```

#### 4.4 OAuthService Implementation
**File:** `Infrastructure/Services/Auth/OAuthService/OAuthService.cs`
- Exchanges authorization code for access token
- Fetches user profile from provider API
- Returns standardized `OAuthUserInfo`
- Handles Google and GitHub-specific API differences

---

### Phase 5: Application Layer - Commands

#### 5.1 OAuthSignInCommand
**File:** `Application/Features/Auth/Commands/OAuthSignInCommand.cs`

**Logic Flow:**
1. Call `oAuthService.GetUserInfoAsync(Provider, Code, ct)` → get provider user info
2. Check if `AuthProvider` exists with this `(Provider, ProviderUserId)`
   - **If exists:** Get associated user, generate JWT
   - **If not exists:** Check if user with this email exists
     - **If user exists:** Create new `AuthProvider` linking to existing user
     - **If user doesn't exist:** Create new user + `AuthProvider`, generate JWT
3. Return `Result<TokenOutput>.Success(token)`

#### 5.2 OAuthLinkAccountCommand
**File:** `Application/Features/Auth/Commands/OAuthLinkAccountCommand.cs`

**Logic Flow:**
1. Check if user already linked to this provider
2. Check if provider account already linked to another user
3. Create new `AuthProvider` linked to user
4. Return success

---

### Phase 6: Presentation Layer

#### 6.1 Authentication Configuration
**File:** `Presentation/Api/Extensions/ApiServiceCollectionExtensions.cs`
```csharp
services.AddAuthentication()
    .AddJwtBearer(...)
    .AddGoogle("Google", options => {
        options.ClientId = config["OAuth:Google:ClientId"];
        options.ClientSecret = config["OAuth:Google:ClientSecret"];
        options.CallbackPath = "/api/auth/oauth/google/callback";
    })
    .AddGitHub("GitHub", options => {
        options.ClientId = config["OAuth:GitHub:ClientId"];
        options.ClientSecret = config["OAuth:GitHub:ClientSecret"];
        options.CallbackPath = "/api/auth/oauth/github/callback";
    });
```

#### 6.2 API Endpoints
**File:** `Presentation/Api/ApiEndpoints.cs`
```csharp
public static class Auth
{
    public const string OAuthGoogle = $"{BaseUrl}/oauth/google";
    public const string OAuthGitHub = $"{BaseUrl}/oauth/github";
    public const string OAuthCallback = $"{BaseUrl}/oauth/callback";
    public const string OAuthLinkAccount = $"{BaseUrl}/oauth/link-account";
}
```

#### 6.3 Controller Actions
**File:** `Presentation/Api/Controllers/Auth/AuthController.cs`
- `OAuthGoogle()` - Redirects to Google
- `OAuthGitHub()` - Redirects to GitHub
- `OAuthGoogleCallback()` - Handles Google callback
- `OAuthGitHubCallback()` - Handles GitHub callback
- `OAuthLinkAccount()` - Links OAuth to existing authenticated user

---

### Phase 7: Configuration

#### 7.1 appsettings.json
**File:** `Presentation/Api/appsettings.json`
```json
"OAuth": {
  "Google": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret"
  },
  "GitHub": {
    "ClientId": "your-github-client-id",
    "ClientSecret": "your-github-client-secret"
  }
}
```

#### 7.2 NuGet Packages Added
- `Microsoft.AspNetCore.Authentication.Google` (10.0.1)
- `AspNet.Security.OAuth.GitHub` (10.0.0)
- `Microsoft.Extensions.Http` (for IHttpClientFactory)

---

## OAuth Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/auth/oauth/google` | GET | Redirects to Google OAuth |
| `/api/auth/oauth/google/callback` | GET | Handles Google OAuth callback |
| `/api/auth/oauth/github` | GET | Redirects to GitHub OAuth |
| `/api/auth/oauth/github/callback` | GET | Handles GitHub OAuth callback |
| `/api/auth/oauth/link-account` | POST | Links OAuth account to authenticated user |

---

## Setup Instructions

### 1. Google OAuth Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Navigate to **APIs & Services** → **Credentials**
4. Create **OAuth Client ID**
   - Application type: Web application
   - Authorized redirect URIs: `https://yourdomain.com/api/auth/oauth/google/callback`
5. Copy **Client ID** and **Client Secret** to `appsettings.json`

### 2. GitHub OAuth Setup

1. Go to [GitHub Developer Settings](https://github.com/settings/developers)
2. Create **New OAuth App**
3. Fill in details:
   - Homepage URL: `https://yourdomain.com`
   - Authorization callback URL: `https://yourdomain.com/api/auth/oauth/github/callback`
4. Copy **Client ID** and **Client Secret** to `appsettings.json`

### 3. Apply Database Migration

```bash
dotnet ef database update --startup-project CleanArchTemplate.Presentation.Api --project CleanArchTemplate.Infrastructure
```

---

## User Flow Examples

### New User Signs Up with Google
1. User clicks "Sign in with Google"
2. Redirected to Google → User logs in → Returns to callback with code
3. System creates new user with Google info
4. System creates AuthProvider linking user to Google
5. System returns JWT token
6. User can now sign in with Google anytime

### Existing User Links GitHub Account
1. User is logged in
2. User goes to Account Settings → Link GitHub
3. Redirected to GitHub → User logs in → Returns to callback
4. System creates AuthProvider linking GitHub to existing user
5. User can now sign in with GitHub too

### User Signs In with Already-Linked Google
1. User clicks "Sign in with Google"
2. Redirected to Google → User logs in → Returns to callback
3. System finds existing AuthProvider for this Google account
4. System returns JWT for associated user

---

## Security Considerations

1. **Single-use codes**: Authorization codes are exchanged immediately for tokens
2. **Email verification**: OAuth users are marked as email verified (provider verified them)
3. **Account linking validation**: Cannot link a provider to multiple users
4. **Cascade delete**: Deleting a user removes all linked OAuth accounts
5. **Unique constraints**: Prevents duplicate OAuth accounts

---

## Future Enhancements

- Add more OAuth providers (Microsoft, Twitter, Facebook, etc.)
- Add "Unlink OAuth Account" functionality
- Add "Set Password" for OAuth-only users
- Add OAuth provider avatars/profile pictures
- Add login history tracking
