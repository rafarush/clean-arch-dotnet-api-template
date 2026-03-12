using MediatR;

namespace CleanArchTemplate.Application.Abstractions.Cqrs.Query;

public interface IQuery<out TResponse> : IRequest<TResponse>;
