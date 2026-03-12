using MediatR;

namespace CleanArchTemplate.Application.Abstractions.Cqrs.Command;

public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;