namespace CleanArchTemplate.SharedKernel.Models.General.Output;

public record PaginatedOutput<T>(IEnumerable<T> Items, int Total);