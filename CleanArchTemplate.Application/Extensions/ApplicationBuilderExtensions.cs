using CleanArchTemplate.Application.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchTemplate.Application.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            
        await initializer.InitializeAsync();
        await initializer.SeedAsync();
            
        return app;
    }
}