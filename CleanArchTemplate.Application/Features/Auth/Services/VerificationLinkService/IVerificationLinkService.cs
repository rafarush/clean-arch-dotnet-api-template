using CleanArchTemplate.Application.Features.Auth.Services.VerificationLinkService.Models;

namespace CleanArchTemplate.Application.Features.Auth.Services.VerificationLinkService;

public interface IVerificationLinkService
{
    string GenerateLink(Domain.User.User user);
    LinkInfo? ParseLink(string linkInfoString);
    int GetLinkLifeInMinutes();
    bool IsLinkInfoValid(Domain.User.User user, LinkInfo linkInfo);
}