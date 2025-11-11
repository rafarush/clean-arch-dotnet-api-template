using CleanArchTemplate.SharedKernel.Models.Output;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CleanArchTemplate.Aplication.Middleware;

public class ValidatorMapperMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var validationFailureOutput = new ValidationFailureOutput
            {
                Errors = ex.Errors.Select(x=> new ValidationOutput
                {
                    PropertyName = x.PropertyName,
                    Message = x.ErrorMessage
                })
            };
            
            await context.Response.WriteAsJsonAsync(validationFailureOutput);
        }
    }
}