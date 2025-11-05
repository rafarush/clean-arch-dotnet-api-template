namespace CleanArchTemplate.SharedKernel.Models.Input;

public class PaginatedInput
{
    public required string OffsetField { get; set; }

    public required int OffsetPage { get; set; }

    public required int Limit { get; set; }

    public virtual bool IsOffsetFieldValid() => true;
}