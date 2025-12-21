using MediatR;

namespace CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;

public interface IQuery<out TResponse> : IRequest<TResponse>;
