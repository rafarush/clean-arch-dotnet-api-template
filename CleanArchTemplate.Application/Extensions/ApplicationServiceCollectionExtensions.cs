using System.Reflection;
using CleanArchTemplate.Application;
using CleanArchTemplate.Application.Features.Auth.Services;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchTemplate.Application.Extensions;

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