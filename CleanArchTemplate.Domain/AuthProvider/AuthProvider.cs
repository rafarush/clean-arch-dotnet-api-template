using CleanArchTemplate.Domain.Abstractions.Primitives;

namespace CleanArchTemplate.Domain.AuthProvider;

public class AuthProvider : BaseEntity
{
    public required Guid UserId { get; set; }
    public required OAuthProviderType Provider { get; set; }
    public required string ProviderUserId { get; set; }
    public User.User User { get; set; } = null!;
}
