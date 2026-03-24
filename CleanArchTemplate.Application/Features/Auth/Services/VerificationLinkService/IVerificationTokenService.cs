using CleanArchTemplate.Application.Features.Auth.Services.VerificationLinkService.Models;

namespace CleanArchTemplate.Application.Features.Auth.Services.VerificationLinkService;

public interface IVerificationTokenService
{
    string GenerateLink(Domain.User.User user);
    TokenInfo? ParseToken(string token);
    int GetTokenLifeInMinutes();
    bool IsTokenValid(Domain.User.User user, TokenInfo tokenInfo);
}