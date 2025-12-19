namespace CleanArchTemplate.Aplication.Features.Security.Policy.Models.Output;

public class PolicyOutput
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? UpdatedAt { get; set; }
}