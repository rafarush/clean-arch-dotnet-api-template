using System.Text;
using System.Text.Json;
using CleanArchTemplate.Application.Services.Auth.VerificationLinkService;
using CleanArchTemplate.Application.Services.Auth.VerificationLinkService.Options;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService.Models;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.Extensions.Options;

namespace CleanArchTemplate.Application.Services.Auth.VerificationTokenService;

public class VerificationTokenService(
    IOptions<VerificationTokenOptions> options,
    AppDbContext db
    ) : IVerificationTokenService
{
    public bool IsTokenValid(Domain.User.User user, TokenInfo tokenInfo, TokenMotive tokenMotive)
    {
        if (tokenInfo.Expiration < DateTimeOffset.UtcNow)
            return false;
        var isTokenValid = tokenMotive switch
        {
            TokenMotive.ConfirmEmail => CheckConfirmationCode(user, tokenInfo),
            TokenMotive.ResetPassword => CheckResetPasswordCode(user, tokenInfo),
            _ => false
        };
        return isTokenValid;
    }

    public int GetTokenLifeInMinutes() => options.Value.ExpirationInMinutes;
    
    public string GenerateLink(Domain.User.User user, TokenMotive tokenMotive)
    {
        var code = Guid.NewGuid().ToString()[..7];
        switch (tokenMotive)
        {
            case TokenMotive.ConfirmEmail:
                user.ConfirmationCode = code;
                break;
            case TokenMotive.ResetPassword: 
                user.ResetPasswordCodes.Add(code);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tokenMotive), tokenMotive, null);
        }
        var tokenInfo = new TokenInfo
        {
            UserId = user.Id,
            Code = code,
            Expiration = DateTimeOffset.UtcNow.AddMinutes(options.Value.ExpirationInMinutes).DateTime
        };
        var json = JsonSerializer.Serialize(tokenInfo);
        var bytes = Encoding.UTF8.GetBytes(json);
        var tokenInfoString = Convert.ToBase64String(bytes);
        return $"{options.Value.RedirectTo}/{tokenInfoString}";
    }

    public TokenInfo? ParseToken(string token)
    {
        try
        {
            var bytes = Convert.FromBase64String(token);
            var json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<TokenInfo>(json);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private bool CheckConfirmationCode(Domain.User.User user, TokenInfo tokenInfo)
    {
        if (string.IsNullOrEmpty(user.ConfirmationCode))
            return false;
        return tokenInfo.Code == user.ConfirmationCode;
    }

    private bool CheckResetPasswordCode(Domain.User.User user, TokenInfo tokenInfo)
    {
        if (user.ResetPasswordCodes.Any())
            return false;

        return user.ResetPasswordCodes.Contains(tokenInfo.Code);
    }
}