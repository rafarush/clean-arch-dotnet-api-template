namespace CleanArchTemplate.SharedKernel.Models.Security.Role.Output;

public class RoleDetailsOutput
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string? Description { get; set; }
    // TODO refactor List of Policy by a List of the PolicyOutput DTO
    public required List<Domain.Security.Policy> Policies { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? UpdatedAt { get; set; }
    public required bool IsDeleted { get; set; }
}