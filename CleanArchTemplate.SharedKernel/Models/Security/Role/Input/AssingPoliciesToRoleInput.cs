namespace CleanArchTemplate.SharedKernel.Models.Security.Role.Input;

public class AssingPoliciesToRoleInput
{
    public required List<Guid> PoliciesIds { get; set; }
}