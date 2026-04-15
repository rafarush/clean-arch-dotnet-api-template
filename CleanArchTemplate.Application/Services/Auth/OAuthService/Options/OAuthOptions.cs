namespace CleanArchTemplate.Application.Services.Auth.OAuthService.Options;

public class OAuthOptions
{
    public const string Section = "OAuth";
    public GoogleOAuthOptions Google { get; set; } = new();
    public GitHubOAuthOptions GitHub { get; set; } = new();
}

public class GoogleOAuthOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class GitHubOAuthOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
