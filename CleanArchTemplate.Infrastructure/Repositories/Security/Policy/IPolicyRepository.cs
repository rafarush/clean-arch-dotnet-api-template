namespace CleanArchTemplate.Infrastructure.Repositories.Security.Policy;

using Domain.Security;

public interface IPolicyRepository
{
    Task<Guid> CreateAsync(Policy policy, CancellationToken ct);
    Task<bool> UpdateAsync(Policy policy, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<Policy?> GetAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Policy>> GetAllAsync(CancellationToken ct);
}