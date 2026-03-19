namespace CleanArchTemplate.SharedKernel.Models.Security.Role.Input;

public class CreateRoleInput
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<Guid>? Policies { get; set; }
}