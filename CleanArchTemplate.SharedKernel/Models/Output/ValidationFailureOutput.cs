namespace CleanArchTemplate.SharedKernel.Models.Output;

public class ValidationFailureOutput
{
    public required IEnumerable<ValidationOutput> Errors { get; init; }
}