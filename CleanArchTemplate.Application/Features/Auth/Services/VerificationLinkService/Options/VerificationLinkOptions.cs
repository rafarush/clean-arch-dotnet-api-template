namespace CleanArchTemplate.Application.Features.Auth.Services.VerificationLinkService.Options;

public sealed class VerificationLinkOptions
{
    public const string Section = "VerificationLinkOptions";
    public required string RedirectTo { get; set; }
    public required int ExpirationInMinutes { get; set; }
}