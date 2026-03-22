namespace CleanArchTemplate.Domain.Abstractions.Primitives;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } =  DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}