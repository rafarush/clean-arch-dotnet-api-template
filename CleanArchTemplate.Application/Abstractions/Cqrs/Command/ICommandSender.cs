namespace CleanArchTemplate.Application.Abstractions.Cqrs.Command;

public interface ICommandSender
{
    Task<TResponse> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken = default);
}
