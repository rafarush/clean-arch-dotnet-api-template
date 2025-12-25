using MediatR;

namespace CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;

public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;