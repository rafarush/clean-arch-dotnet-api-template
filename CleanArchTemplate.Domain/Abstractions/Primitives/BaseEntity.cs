namespace CleanArchTemplate.Domain.Abstractions.Primitives;

public abstract class BaseEntity
{
    public required Guid Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}