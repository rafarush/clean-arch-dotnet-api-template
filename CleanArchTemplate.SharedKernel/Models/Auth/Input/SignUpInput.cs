namespace CleanArchTemplate.SharedKernel.Models.Auth.Input;

public class SignUpInput
{
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Password { get; set; }
}