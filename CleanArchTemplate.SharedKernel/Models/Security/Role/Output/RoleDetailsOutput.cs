using CleanArchTemplate.SharedKernel.Models.Security.Policy.Output;

namespace CleanArchTemplate.SharedKernel.Models.Security.Role.Output;

public class RoleDetailsOutput
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string? Description { get; set; }
    public required List<PolicyOutput>? Policies { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? UpdatedAt { get; set; }
    public required bool IsDeleted { get; set; }
}