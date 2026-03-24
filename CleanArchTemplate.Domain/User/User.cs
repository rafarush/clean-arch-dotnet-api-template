using CleanArchTemplate.Domain.Abstractions.Primitives;
using CleanArchTemplate.Domain.Security;

namespace CleanArchTemplate.Domain.User;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required bool EmailVerified { get; set; }
    public string? ConfirmationCode { get; set; }
    public List<Role> Roles { get; set; } = [];
}