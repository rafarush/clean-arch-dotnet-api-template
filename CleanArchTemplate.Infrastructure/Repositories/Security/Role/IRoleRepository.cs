namespace CleanArchTemplate.Infrastructure.Repositories.Security.Role;

using Domain.Security;

public interface IRoleRepository
{
    Task<Guid> CreateAsync(Role role, CancellationToken ct);
    Task<bool> UpdateAsync(Role role, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<Role?> GetAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken ct);
}