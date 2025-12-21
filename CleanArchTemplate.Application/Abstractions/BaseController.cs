using CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Aplication.Abstractions;

[ApiController]
public abstract class BaseApiController(
    ICommandSender commandSender, 
    IQuerySender querySender) : ControllerBase
{
    
    protected async Task<IActionResult> HandleCommandAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken ct)
        where TCommand : ICommand<TResult>
    {
        var result = await commandSender.SendAsync(command, ct);
        return Ok(result);
    }
    
    protected async Task<IActionResult> HandleCreateCommandAsync<TCommand, TResult>(
        TCommand command,
        string getActionName,
        Func<TResult, Guid> getId,
        Func<TResult, object?> getOutput,
        CancellationToken ct)
        where TCommand : ICommand<TResult>
    {
        var result = await commandSender.SendAsync(command, ct);

        var id = getId(result);
        var output = getOutput(result);

        return CreatedAtAction(getActionName, new { id }, output);
    }

    protected async Task<IActionResult> HandleQueryAsync<TQuery, TResult>(
        TQuery query,
        CancellationToken ct)
        where TQuery : IQuery<TResult>
    {
        var result = await querySender.SendAsync(query, ct);
        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
