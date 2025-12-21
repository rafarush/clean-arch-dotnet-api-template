using System.Reflection;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;
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
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<ICommandSender, CommandSender>();
        services.AddScoped<IQuerySender, QuerySender>();
        
        return services;
    }
}