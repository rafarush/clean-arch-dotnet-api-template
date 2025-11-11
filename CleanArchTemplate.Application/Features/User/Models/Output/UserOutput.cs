namespace CleanArchTemplate.Application.Features.User.Models.Output;

public class UserOutput
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? UpdatedAt { get; set; }
    public required bool IsDeleted { get; set; }
}