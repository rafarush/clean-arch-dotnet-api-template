using System.Net.Http.Headers;
using System.Text.Json;
using CleanArchTemplate.Application.Services.Auth.OAuthService;
using CleanArchTemplate.Application.Services.Auth.OAuthService.Options;
using CleanArchTemplate.Domain.AuthProvider;
using Microsoft.Extensions.Options;

namespace CleanArchTemplate.Application.Services.Auth.OAuthService;

public class OAuthService(IOptions<OAuthOptions> options, IHttpClientFactory httpClientFactory) : IOAuthService
{
    public string GetAuthorizationUrl(OAuthProviderType provider, string state)
    {
        return provider switch
        {
            OAuthProviderType.Google =>
                $"https://accounts.google.com/o/oauth2/v2/auth?client_id={Uri.EscapeDataString(options.Value.Google.ClientId)}&response_type=code&scope={Uri.EscapeDataString("openid email profile")}&redirect_uri={Uri.EscapeDataString(options.Value.Google.RedirectUri)}&state={Uri.EscapeDataString(state)}",
            OAuthProviderType.GitHub =>
                $"https://github.com/login/oauth/authorize?client_id={Uri.EscapeDataString(options.Value.GitHub.ClientId)}&scope={Uri.EscapeDataString("read:user user:email")}&redirect_uri={Uri.EscapeDataString(options.Value.GitHub.RedirectUri)}&state={Uri.EscapeDataString(state)}",
            _ => throw new ArgumentOutOfRangeException(nameof(provider))
        };
    }

    public async Task<OAuthUserInfo> GetUserInfoAsync(OAuthProviderType provider, string code, CancellationToken ct)
    {
        return provider switch
        {
            OAuthProviderType.Google => await GetGoogleUserInfoAsync(code, ct),
            OAuthProviderType.GitHub => await GetGitHubUserInfoAsync(code, ct),
            _ => throw new ArgumentOutOfRangeException(nameof(provider))
        };
    }

    private async Task<OAuthUserInfo> GetGoogleUserInfoAsync(string code, CancellationToken ct)
    {
        var tokenResponse = await ExchangeCodeForTokenAsync(
            "https://oauth2.googleapis.com/token",
            new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = options.Value.Google.ClientId,
                ["client_secret"] = options.Value.Google.ClientSecret,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = options.Value.Google.RedirectUri
            },
            ct);

        var accessToken = tokenResponse.GetProperty("access_token").GetString()
                          ?? throw new InvalidOperationException("Google access token not found");

        var userInfoResponse = await FetchUserInfoAsync(
            "https://openidconnect.googleapis.com/v1/userinfo",
            accessToken,
            ct);

        var email = userInfoResponse.GetProperty("email").GetString() ?? throw new InvalidOperationException("Email not found");
        var name = userInfoResponse.TryGetProperty("given_name", out var givenName)
            ? givenName.GetString() ?? ""
            : userInfoResponse.GetProperty("name").GetString() ?? "";
        var lastName = userInfoResponse.TryGetProperty("family_name", out var familyName)
            ? familyName.GetString()
            : null;
        var sub = userInfoResponse.GetProperty("sub").GetString() ?? throw new InvalidOperationException("Sub not found");

        return new OAuthUserInfo(sub, email, name, lastName);
    }

    private async Task<OAuthUserInfo> GetGitHubUserInfoAsync(string code, CancellationToken ct)
    {
        var tokenResponse = await ExchangeCodeForTokenAsync(
            "https://github.com/login/oauth/access_token",
            new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = options.Value.GitHub.ClientId,
                ["client_secret"] = options.Value.GitHub.ClientSecret,
                ["redirect_uri"] = options.Value.GitHub.RedirectUri
            },
            ct,
            requiresJsonResponse: true);

        var accessToken = tokenResponse.GetProperty("access_token").GetString()
                          ?? throw new InvalidOperationException("GitHub access token not found");

        var userInfoResponse = await FetchUserInfoAsync(
            "https://api.github.com/user",
            accessToken,
            ct);

        var id = userInfoResponse.GetProperty("id").GetInt64().ToString();
        var email = userInfoResponse.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
        var fullName = userInfoResponse.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
        var login = userInfoResponse.TryGetProperty("login", out var loginProp) ? loginProp.GetString() ?? "" : "";

        if (string.IsNullOrEmpty(email))
        {
            var emailsResponse = await FetchUserInfoArrayAsync(
                "https://api.github.com/user/emails",
                accessToken,
                ct);

            email = emailsResponse
                .FirstOrDefault(e =>
                    e.TryGetProperty("primary", out var primary) && primary.GetBoolean() &&
                    e.TryGetProperty("verified", out var verified) && verified.GetBoolean())
                .GetPropertyOrDefault("email")
                ?? emailsResponse
                    .FirstOrDefault(e =>
                        e.TryGetProperty("verified", out var verified) && verified.GetBoolean())
                    .GetPropertyOrDefault("email");
        }

        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidOperationException("GitHub account does not provide a verified email");

        var (firstName, lastName) = SplitName(fullName, login);
        return new OAuthUserInfo(id, email, firstName, lastName);
    }

    private async Task<JsonElement> ExchangeCodeForTokenAsync(
        string url,
        Dictionary<string, string> body,
        CancellationToken ct,
        bool requiresJsonResponse = false)
    {
        using var client = httpClientFactory.CreateClient();
        var content = new FormUrlEncodedContent(body);

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };

        if (requiresJsonResponse)
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        using var doc = JsonDocument.Parse(responseBody);
        return doc.RootElement.Clone();
    }

    private async Task<JsonElement> FetchUserInfoAsync(string url, string accessToken, CancellationToken ct)
    {
        using var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("CleanArchTemplate/1.0");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        using var doc = JsonDocument.Parse(responseBody);
        return doc.RootElement.Clone();
    }

    private async Task<JsonElement[]> FetchUserInfoArrayAsync(string url, string accessToken, CancellationToken ct)
    {
        var element = await FetchUserInfoAsync(url, accessToken, ct);
        if (element.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException("Unexpected OAuth response format");

        return element.EnumerateArray().ToArray();
    }

    private static (string FirstName, string? LastName) SplitName(string fullName, string fallback)
    {
        var value = string.IsNullOrWhiteSpace(fullName) ? fallback : fullName;
        var parts = value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0)
            return (fallback, null);

        if (parts.Length == 1)
            return (parts[0], null);

        return (parts[0], string.Join(' ', parts.Skip(1)));
    }
}

internal static class JsonElementExtensions
{
    public static string? GetPropertyOrDefault(this JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var value)
            ? value.GetString()
            : null;
    }
}
