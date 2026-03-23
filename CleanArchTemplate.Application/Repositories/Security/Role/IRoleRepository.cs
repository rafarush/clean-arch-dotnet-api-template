namespace CleanArchTemplate.Application.Repositories.Security.Role;

public interface IRoleRepository
{
    Task<Guid> CreateAsync(Domain.Security.Role role, CancellationToken ct);
    Task<bool> UpdateAsync(Domain.Security.Role role, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<Domain.Security.Role?> GetAsync(Guid id, CancellationToken ct);
    Task<Domain.Security.Role?> GetByNameAsync(string name, CancellationToken ct);
    Task<IEnumerable<Domain.Security.Role>> GetAllAsync(CancellationToken ct);
    Task<bool> ExistsAsync(string name, CancellationToken ct);
    Task<Domain.Security.Role> AssignPoliciesToRoleAsync(Domain.Security.Role role, List<Domain.Security.Policy> policies, CancellationToken ct);
    Task<List<Domain.Security.Role>> GetByIdsAsync(List<Guid> id, CancellationToken ct);
}