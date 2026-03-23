using CleanArchTemplate.Domain.Abstractions.Primitives;

namespace CleanArchTemplate.Domain.Security;

public class Policy : BaseEntity
{
    public required string Name { get; set; }
    public List<Role>? Roles { get; set; }
}