using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.SharedKernel.Models.General.Output;

namespace CleanArchTemplate.Application.Abstractions.Models.Input;

public record BasePaginatedQuery
{
    public required string OffsetField { get; set; }

    public required int OffsetPage { get; set; }

    public required int Limit { get; set; }
    
    public bool HasPagination { get; set; } = true;
    
    public bool IsAsc { get; set; } = true;
    public virtual bool IsOffsetFieldValid() => true;
}

public abstract record BasePaginatedQuery<T> : BasePaginatedQuery, IQuery<Result<PaginatedOutput<T>>>;

public abstract record BasePaginatedQueryExport: BasePaginatedQuery, IQuery<byte[]>;