using MediatR;

namespace CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;

public interface IQueryHandler<in TQuery, TResponse> 
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;