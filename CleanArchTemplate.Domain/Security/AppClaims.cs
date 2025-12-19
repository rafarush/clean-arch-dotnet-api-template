using System.Security.Claims;

namespace CleanArchTemplate.Domain.Security;

public static class AppClaims
{
    public const string UserId = ClaimTypes.NameIdentifier;
    public const string Role = ClaimTypes.Role;
    public const string Policies = "Policies";
    public const string Email = ClaimTypes.Email;
}