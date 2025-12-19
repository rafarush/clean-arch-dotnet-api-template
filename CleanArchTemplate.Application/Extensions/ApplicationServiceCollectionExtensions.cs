using CleanArchTemplate.Aplication.Features.Auth.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchTemplate.Aplication.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}