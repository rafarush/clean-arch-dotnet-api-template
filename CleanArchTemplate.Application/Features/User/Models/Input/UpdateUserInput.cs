namespace CleanArchTemplate.Aplication.Features.User.Models.Input;

public class UpdateUserInput
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; } 
    public List<Guid> RoleIds { get; set; } = [];
    public List<Guid> PolicyIds { get; set; } = [];
}