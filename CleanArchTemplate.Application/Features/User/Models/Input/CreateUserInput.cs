namespace CleanArchTemplate.Application.Features.User.Models.Input;

public class CreateUserInput
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}