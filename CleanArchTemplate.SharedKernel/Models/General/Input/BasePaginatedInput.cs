using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchTemplate.SharedKernel.Models.General.Input;

public class BasePaginatedInput
{
    public virtual required string OffsetField { get; set; }
    public required int OffsetPage { get; set; }
    public required int Limit { get; set; }
    public bool HasPagination { get; set; } = true;
    public bool IsAsc { get; set; } = true;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? Id { get; set; }
}