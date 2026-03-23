using CleanArchTemplate.Application.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace CleanArchTemplate.Application.Extensions;

public static class WebApplicationExtensions
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
}