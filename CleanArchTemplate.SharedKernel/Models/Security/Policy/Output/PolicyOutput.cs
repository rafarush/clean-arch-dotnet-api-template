namespace CleanArchTemplate.SharedKernel.Models.Security.Policy.Output;

public class PolicyOutput
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
}