using System.Net.Http.Headers;
using System.Text.Json;
using CleanArchTemplate.Application.Services.Auth.OAuthService;
using CleanArchTemplate.Application.Services.Auth.OAuthService.Options;
using CleanArchTemplate.Domain.AuthProvider;
using Microsoft.Extensions.Options;

namespace CleanArchTemplate.Application.Services.Auth.OAuthService;

public class OAuthService(IOptions<OAuthOptions> options) : IOAuthService
{
    public string GetAuthorizationUrl(OAuthProviderType provider)
    {
        return provider switch
        {
            OAuthProviderType.Google => $"https://accounts.google.com/o/oauth2/v2/auth?client_id={options.Value.Google.ClientId}&response_type=code&scope=openid%20email%20profile&redirect_uri=",
            OAuthProviderType.GitHub => $"https://github.com/login/oauth/authorize?client_id={options.Value.GitHub.ClientId}&scope=user:email",
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
                ["redirect_uri"] = ""
            },
            ct);

        var accessToken = tokenResponse.GetProperty("access_token").GetString();

        var userInfoResponse = await FetchUserInfoAsync(
            "https://www.googleapis.com/oauth2/v2/userinfo",
            accessToken!,
            ct);

        var email = userInfoResponse.GetProperty("email").GetString() ?? throw new InvalidOperationException("Email not found");
        var name = userInfoResponse.GetProperty("name").GetString() ?? "";
        var sub = userInfoResponse.GetProperty("sub").GetString() ?? throw new InvalidOperationException("Sub not found");

        return new OAuthUserInfo(sub, email, name, null);
    }

    private async Task<OAuthUserInfo> GetGitHubUserInfoAsync(string code, CancellationToken ct)
    {
        var tokenResponse = await ExchangeCodeForTokenAsync(
            "https://github.com/login/oauth/access_token",
            new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = options.Value.GitHub.ClientId,
                ["client_secret"] = options.Value.GitHub.ClientSecret
            },
            ct);

        var accessToken = tokenResponse.GetProperty("access_token").GetString();

        var userInfoResponse = await FetchUserInfoAsync(
            "https://api.github.com/user",
            accessToken!,
            ct);

        var id = userInfoResponse.GetProperty("id").GetInt64().ToString();
        var email = userInfoResponse.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
        var name = userInfoResponse.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
        var login = userInfoResponse.GetProperty("login").GetString() ?? "";

        if (string.IsNullOrEmpty(email))
        {
            var emailsResponse = await FetchUserInfoAsync(
                "https://api.github.com/user/emails",
                accessToken!,
                ct);

            email = emailsResponse.EnumerateArray()
                .FirstOrDefault(e => e.TryGetProperty("primary", out var primary) && primary.GetBoolean())
                .GetProperty("email").GetString();
        }

        return new OAuthUserInfo(id, email ?? login, name, null);
    }

    private async Task<JsonElement> ExchangeCodeForTokenAsync(string url, Dictionary<string, string> body, CancellationToken ct)
    {
        using var client = new HttpClient();
        var content = new FormUrlEncodedContent(body);

        var response = await client.PostAsync(url, content, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        using var doc = JsonDocument.Parse(responseBody);
        return doc.RootElement;
    }

    private async Task<JsonElement> FetchUserInfoAsync(string url, string accessToken, CancellationToken ct)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("CleanArchTemplate/1.0");

        var response = await client.GetAsync(url, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        using var doc = JsonDocument.Parse(responseBody);
        return doc.RootElement;
    }
}
