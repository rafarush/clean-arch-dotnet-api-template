using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Application.Abstractions;

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
        return HandleResult(result);
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
            return HandleResult(result);
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
        return HandleResult(result);
    }

    private IActionResult HandleResult<T>(T result)
    {
        // Check if result is any Result<> type using reflection
        var resultType = result?.GetType();
        if (resultType != null && resultType.IsGenericType && 
            resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var isSuccessProperty = resultType.GetProperty(nameof(Result<object>.IsSuccess));
            var isSuccess = (bool)isSuccessProperty!.GetValue(result)!;
            
            var statusCodeProperty = resultType.GetProperty(nameof(Result<object>.StatusCode));
            var customStatusCode = (int?)statusCodeProperty!.GetValue(result);
            
            if (!isSuccess)
            {
                var errorProperty = resultType.GetProperty(nameof(Result<object>.Error));
                var messageProperty = resultType.GetProperty(nameof(Result<object>.Message));
                var errorTypeProperty = resultType.GetProperty(nameof(Result<object>.ErrorType));
                
                var error = (string)errorProperty!.GetValue(result)!;
                var message = (string?)messageProperty!.GetValue(result);
                var errorType = (ErrorType)errorTypeProperty!.GetValue(result)!;
                
                var statusCode = customStatusCode ?? errorType switch
                {
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Validation => StatusCodes.Status400BadRequest,
                    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                    ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    ErrorType.BusinessRule => StatusCodes.Status422UnprocessableEntity,
                    _ => StatusCodes.Status500InternalServerError
                };
                
                var problemDetails = new ProblemDetails
                {
                    Title = error,
                    Detail = message ?? error,
                    Status = statusCode,
                    Type = errorType.ToString()
                };

                return new ObjectResult(problemDetails)
                {
                    StatusCode = statusCode
                };
            }

            var valueProperty = resultType.GetProperty(nameof(Result<object>.Value));
            var value = valueProperty!.GetValue(result);
            
            var successStatusCode = customStatusCode ?? StatusCodes.Status200OK;
            
            return successStatusCode switch
            {
                204 => NoContent(),
                201 => new ObjectResult(value) { StatusCode = 201 },
                _ => Ok(value)
            };
        }

        return Ok(result);
    }
}
