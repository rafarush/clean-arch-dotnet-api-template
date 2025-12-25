namespace CleanArchTemplate.SharedKernel.Models.General.Output;

public class ValidationOutput
{
    public required string PropertyName { get; init; }
    public required string Message { get; init; }
}