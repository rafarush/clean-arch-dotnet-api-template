namespace CleanArchTemplate.SharedKernel.Models.General.Output;

public class ValidationFailureOutput
{
    public required IEnumerable<ValidationOutput> Errors { get; init; }
}