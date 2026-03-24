using System.Text;
using System.Text.Json;
using CleanArchTemplate.Application.Features.Auth.Services.VerificationLinkService.Models;
using CleanArchTemplate.Application.Features.Auth.Services.VerificationLinkService.Options;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.Extensions.Options;

namespace CleanArchTemplate.Application.Features.Auth.Services.VerificationLinkService;

public class VerificationLinkService(
    IOptions<VerificationLinkOptions> options) : IVerificationLinkService
{
    public bool IsLinkInfoValid(Domain.User.User user, LinkInfo linkInfo)
    {
        if (linkInfo.Expiration < DateTimeOffset.Now)
            return false;
        
        if (string.IsNullOrEmpty(user.ConfirmationCode))
            return false;

        return linkInfo.Code == user.ConfirmationCode;
    }

    public int GetLinkLifeInMinutes() => options.Value.ExpirationInMinutes;
    
    public string GenerateLink(Domain.User.User user)
    {
        user.ConfirmationCode = Guid.NewGuid().ToString()[..7];
        var linkInfo = new LinkInfo
        {
            UserId = user.Id,
            Code = user.ConfirmationCode,
            Expiration = DateTimeOffset.Now.AddMinutes(options.Value.ExpirationInMinutes).DateTime
        };
        var json = JsonSerializer.Serialize(linkInfo);
        var bytes = Encoding.UTF8.GetBytes(json);
        var linkInfoString = Convert.ToBase64String(bytes);
        return $"{options.Value.RedirectTo}/{linkInfoString}";
    }

    public LinkInfo? ParseLink(string linkInfoString)
    {
        try
        {
            var bytes = Convert.FromBase64String(linkInfoString);
            var json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<LinkInfo>(json);
        }
        catch (Exception)
        {
            return null;
        }
    }
}