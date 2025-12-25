namespace CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;

using MediatR;

public sealed class QuerySender(ISender sender) : IQuerySender
{
    public Task<TResponse> SendAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken ct)
        => sender.Send(query, ct);
}
