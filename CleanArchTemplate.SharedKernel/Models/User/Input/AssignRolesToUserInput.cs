namespace CleanArchTemplate.SharedKernel.Models.User.Input;

public class AssignRolesToUserInput
{
    public required List<Guid> RoleIds { get; set; }
}