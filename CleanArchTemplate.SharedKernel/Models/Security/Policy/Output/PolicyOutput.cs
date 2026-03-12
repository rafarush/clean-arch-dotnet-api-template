namespace CleanArchTemplate.SharedKernel.Models.Security.Policy.Output;

public class PolicyOutput
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? UpdatedAt { get; set; }
}