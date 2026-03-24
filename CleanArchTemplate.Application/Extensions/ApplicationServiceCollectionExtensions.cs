using System.Reflection;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using CleanArchTemplate.Application.Persistence;
using CleanArchTemplate.Application.Repositories.Security.Policy;
using CleanArchTemplate.Application.Repositories.Security.Role;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.JwtService;
using CleanArchTemplate.Application.Services.Auth.PasswordHashService;
using CleanArchTemplate.Application.Services.Auth.PasswordHashService.Options;
using CleanArchTemplate.Application.Services.Auth.PermissionService;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CleanArchTemplate.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        services.AddScoped<IJwtService, JwtService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<ICommandSender, CommandSender>();
        services.AddScoped<IQuerySender, QuerySender>();
        
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        
        
        // Services
        services.Configure<PasswordHashServiceOptions>(config.GetSection(PasswordHashServiceOptions.Section));
        services.AddSingleton<IPasswordHashService, PasswordHashService>();
        
        // Custom Auth Service
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        
        return services;
    }
}