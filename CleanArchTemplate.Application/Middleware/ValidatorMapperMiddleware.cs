using System.Text.Json;
using CleanArchTemplate.SharedKernel.Models.General.Output;
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
            context.Response.ContentType = "application/json";
            
            var validationFailureOutput = new ValidationFailureOutput
            {
                Errors = ex.Errors.Select(x=> new ValidationOutput
                {
                    PropertyName = x.PropertyName,
                    Message = x.ErrorMessage
                }).ToList()
            };
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var json = JsonSerializer.Serialize(validationFailureOutput, options);
            await context.Response.WriteAsync(json);
            // await context.Response.WriteAsJsonAsync(validationFailureOutput);
        }
    }
}