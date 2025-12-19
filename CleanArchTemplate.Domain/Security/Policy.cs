using CleanArchTemplate.Domain.Abstractions.Primitives;

namespace CleanArchTemplate.Domain.Security;

public class Policy : BaseEntity
{
    public required string Name { get; set; }
    public List<Role>? Roles { get; set; }


    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
            
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}