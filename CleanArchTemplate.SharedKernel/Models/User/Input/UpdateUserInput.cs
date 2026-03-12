namespace CleanArchTemplate.SharedKernel.Models.User.Input;

public class UpdateUserInput
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public List<Guid> RoleIds { get; set; } = [];
}