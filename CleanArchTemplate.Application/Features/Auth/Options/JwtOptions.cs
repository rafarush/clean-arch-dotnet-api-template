namespace CleanArchTemplate.Application.Features.Auth.Options;

public sealed class JwtOptions
{
    public required string Audience { get; set; }
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
}