namespace CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;

using MediatR;

public sealed class CommandSender(ISender sender) : ICommandSender
{
    public Task<TResponse> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken ct)
    {
        return sender.Send(command, ct);
    }
}
