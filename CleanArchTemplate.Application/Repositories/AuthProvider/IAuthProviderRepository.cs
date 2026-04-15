using AuthProviderEntity = CleanArchTemplate.Domain.AuthProvider.AuthProvider;
using CleanArchTemplate.Domain.AuthProvider;

namespace CleanArchTemplate.Application.Repositories.AuthProvider;

public interface IAuthProviderRepository
{
    Task<AuthProviderEntity?> GetByProviderAsync(OAuthProviderType provider, string providerUserId, CancellationToken ct);
    Task<AuthProviderEntity?> GetByUserAndProviderAsync(Guid userId, OAuthProviderType provider, CancellationToken ct);
    Task<Guid> CreateAsync(AuthProviderEntity authProvider, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> DeleteByUserAndProviderAsync(Guid userId, OAuthProviderType provider, CancellationToken ct);
}
