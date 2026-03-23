namespace CleanArchTemplate.Application.Repositories.Security.Policy;

public interface IPolicyRepository
{
    Task<Guid> CreateAsync(Domain.Security.Policy policy, CancellationToken ct);
    Task<bool> UpdateAsync(Domain.Security.Policy policy, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<Domain.Security.Policy?> GetAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Domain.Security.Policy>> GetAllAsync(CancellationToken ct);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
    Task<List<Domain.Security.Policy>> GetByIdsAsync(List<Guid> id, CancellationToken ct);
}