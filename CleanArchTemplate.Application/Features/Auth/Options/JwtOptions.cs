namespace CleanArchTemplate.Aplication.Features.Auth.Options;

public sealed class JwtOptions
{
    public required string Audience { get; set; }
    public required string Key { get; set; }
    public required string Issuer { get; set; }
}