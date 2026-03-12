using MediatR;

namespace CleanArchTemplate.Application.Abstractions.Cqrs.Command;

public interface ICommand<out TResponse> : IRequest<TResponse>;