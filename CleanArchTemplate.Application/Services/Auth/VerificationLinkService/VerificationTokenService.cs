using System.Text;
using System.Text.Json;
using CleanArchTemplate.Application.Services.Auth.VerificationLinkService.Models;
using CleanArchTemplate.Application.Services.Auth.VerificationLinkService.Options;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.Extensions.Options;

namespace CleanArchTemplate.Application.Services.Auth.VerificationLinkService;

public class VerificationTokenService(
    IOptions<VerificationTokenOptions> options,
    AppDbContext db
    ) : IVerificationTokenService
{
    public bool IsTokenValid(Domain.User.User user, TokenInfo tokenInfo)
    {
        if (tokenInfo.Expiration < DateTimeOffset.UtcNow)
            return false;
        
        if (string.IsNullOrEmpty(user.ConfirmationCode))
            return false;

        return tokenInfo.Code == user.ConfirmationCode;
    }

    public int GetTokenLifeInMinutes() => options.Value.ExpirationInMinutes;
    
    public string GenerateLink(Domain.User.User user)
    {
        user.ConfirmationCode = Guid.NewGuid().ToString()[..7];
        var tokenInfo = new TokenInfo
        {
            UserId = user.Id,
            Code = user.ConfirmationCode,
            Expiration = DateTimeOffset.UtcNow.AddMinutes(options.Value.ExpirationInMinutes).DateTime
        };
        var json = JsonSerializer.Serialize(tokenInfo);
        var bytes = Encoding.UTF8.GetBytes(json);
        var linkInfoString = Convert.ToBase64String(bytes);
        return $"{options.Value.RedirectTo}/{linkInfoString}";
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
}