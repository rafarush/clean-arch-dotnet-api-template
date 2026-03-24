using CleanArchTemplate.Domain.Abstractions.Primitives;

namespace CleanArchTemplate.Domain.Security;

public class Role : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required List<Policy> Policies { get; set; } = [];
    public List<User.User>? Users { get; set; }
}