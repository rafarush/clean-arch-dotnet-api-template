namespace CleanArchTemplate.Application.Features.Auth.Options;

public sealed class JwtOptions
{
    public const string Section = "Jwt";
    public required string Audience { get; set; }
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
}