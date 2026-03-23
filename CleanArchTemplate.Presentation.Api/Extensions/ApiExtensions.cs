using System.Text;
using CleanArchTemplate.Api.Middleware;
using CleanArchTemplate.Application.Extensions;
using CleanArchTemplate.Application.Features.Auth.Options;
using CleanArchTemplate.Application.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CleanArchTemplate.Api.Extensions;

public static class ApiExtensions
{
    public async static void UseSharedMiddlewares(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ValidatorMapperMiddleware>();

        app.MapControllers();
        
        await app.InitializeDatabaseAsync();
    }

    private static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            
        await initializer.InitializeAsync();
        await initializer.SeedAsync();
            
        return app;
    }
}