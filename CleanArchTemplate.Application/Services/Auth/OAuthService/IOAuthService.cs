using CleanArchTemplate.Domain.AuthProvider;

namespace CleanArchTemplate.Application.Services.Auth.OAuthService;

public interface IOAuthService
{
    string GetAuthorizationUrl(OAuthProviderType provider, string state);
    Task<OAuthUserInfo> GetUserInfoAsync(OAuthProviderType provider, string code, CancellationToken ct);
}
