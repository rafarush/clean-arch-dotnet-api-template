using CleanArchTemplate.Application.Services.Auth.VerificationTokenService.Models;

namespace CleanArchTemplate.Application.Services.Auth.VerificationTokenService;

public interface IVerificationTokenService
{
    string GenerateLink(Domain.User.User user, TokenMotive tokenMotive);
    TokenInfo? ParseToken(string token);
    int GetTokenLifeInMinutes();
    bool IsTokenValid(Domain.User.User user, TokenInfo tokenInfo, TokenMotive tokenMotive);
}