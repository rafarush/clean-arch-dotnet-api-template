using CleanArchTemplate.Domain.Security;
using CleanArchTemplate.SharedKernel.Models.General.Output;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;

namespace CleanArchTemplate.Application.Repositories.User;

public interface IUserRepository
{
    Task<Guid> CreateAsync(Domain.Users.User user, CancellationToken ct, List<Role>? roles = null);
    Task<bool> UpdateAsync(Domain.Users.User user, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<Domain.Users.User?> GetAsync(Guid id, CancellationToken ct);
    Task<Domain.Users.User?> GetWithRelationsAsync(Guid id, CancellationToken ct);
    Task<Domain.Users.User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<IEnumerable<Domain.Users.User>> GetAllAsync(CancellationToken ct);
    Task<PaginatedOutput<UserOutput>> SearchUsersAsync(SearchUsersInput usersInput, CancellationToken ct);
    Task<Domain.Users.User> AssignRolesToUserAsync(Domain.Users.User role, List<Role> roles, CancellationToken ct);
}