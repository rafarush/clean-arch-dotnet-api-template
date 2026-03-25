namespace CleanArchTemplate.Application.Services.Auth.VerificationTokenService.Models;

public sealed class TokenInfo
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
    public required DateTime Expiration { get; set; }
}