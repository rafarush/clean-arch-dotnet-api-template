using MediatR;

namespace CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;

public interface ICommand<out TResponse> : IRequest<TResponse>;