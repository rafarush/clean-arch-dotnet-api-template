using CleanArchTemplate.Aplication.Abstractions.Cqrs;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;
using Microsoft.AspNetCore.Http;
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
    
    protected async Task<IActionResult> HandleCreateCommandAsync<TCommand, TOutput>(
        TCommand command,
        string getActionName,
        Func<Result<TOutput>, Guid> getId,
        Func<Result<TOutput>, object?> getOutput,
        CancellationToken ct)
        where TCommand : ICommand<Result<TOutput>>
    {
        var result = await commandSender.SendAsync(command, ct);
        
        if (!result.IsSuccess)
        {
            var problemDetails = new ProblemDetails
            {
                Title = result.Error,
                Detail = result.Message ?? result.Error,
                Status = result.ErrorType switch
                {
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Validation => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status400BadRequest
                },
                Type = result.ErrorType.ToString()
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status!.Value
            };
        }

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
