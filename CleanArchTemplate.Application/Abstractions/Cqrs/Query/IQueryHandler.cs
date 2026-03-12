using MediatR;

namespace CleanArchTemplate.Application.Abstractions.Cqrs.Query;

public interface IQueryHandler<in TQuery, TResponse> 
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;