namespace CleanArchTemplate.Infrastructure.Repositories.User;
using Domain.Users;

public interface IUserRepository
{
    Task<Guid> CreateAsync(User user, CancellationToken ct);
    Task<bool> UpdateAsync(User user, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<User?> GetAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken ct);
}