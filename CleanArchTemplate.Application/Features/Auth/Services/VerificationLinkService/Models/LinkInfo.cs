namespace CleanArchTemplate.Application.Features.Auth.Services.VerificationLinkService.Models;

public sealed class LinkInfo
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
    public required DateTime Expiration { get; set; }
}