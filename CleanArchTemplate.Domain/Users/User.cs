using CleanArchTemplate.Domain.Abstractions.Primitives;

namespace CleanArchTemplate.Domain.Users;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}