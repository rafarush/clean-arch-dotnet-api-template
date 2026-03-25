namespace CleanArchTemplate.Application.Services.Auth.VerificationLinkService.Options;

public sealed class VerificationTokenOptions
{
    public const string Section = "VerificationTokenOptions";
    public required string RedirectTo { get; set; }
    public required int ExpirationInMinutes { get; set; }
}