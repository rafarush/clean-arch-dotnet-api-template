using CleanArchTemplate.Application.Services.Auth.VerificationLinkService.Models;

namespace CleanArchTemplate.Application.Services.Auth.VerificationLinkService;

public interface IVerificationTokenService
{
    string GenerateLink(Domain.User.User user);
    TokenInfo? ParseToken(string token);
    int GetTokenLifeInMinutes();
    bool IsTokenValid(Domain.User.User user, TokenInfo tokenInfo);
}