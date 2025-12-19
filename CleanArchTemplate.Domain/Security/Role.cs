using CleanArchTemplate.Domain.Abstractions.Primitives;
using CleanArchTemplate.Domain.Users;

namespace CleanArchTemplate.Domain.Security;

public class Role : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required List<Policy> Policies { get; set; }
    public List<User>? Users { get; set; }
}