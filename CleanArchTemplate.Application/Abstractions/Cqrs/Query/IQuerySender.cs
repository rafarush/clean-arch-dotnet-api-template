namespace CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;

public interface IQuerySender
{
    Task<TResponse> SendAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken ct);
}