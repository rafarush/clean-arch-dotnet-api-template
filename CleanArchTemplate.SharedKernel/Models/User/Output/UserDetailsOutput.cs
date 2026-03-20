using CleanArchTemplate.SharedKernel.Models.Security.Role.Output;

namespace CleanArchTemplate.SharedKernel.Models.User.Output;

public class UserDetailsOutput
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required List<RoleOutput> Roles { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? UpdatedAt { get; set; }
    public required bool IsDeleted { get; set; }
}