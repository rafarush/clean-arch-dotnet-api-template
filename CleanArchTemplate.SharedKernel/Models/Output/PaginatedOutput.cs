namespace CleanArchTemplate.SharedKernel.Models.Output;

public record PaginatedOutput<T>(IEnumerable<T> Items, int Total);