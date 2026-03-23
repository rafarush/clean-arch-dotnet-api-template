using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using CleanArchTemplate.Infrastructure.Services.Auth;
using CleanArchTemplate.Infrastructure.Services.Auth.Options;
using CleanArchTemplate.Infrastructure.Services.Auth.PasswordHashService;
using CleanArchTemplate.Infrastructure.Services.Auth.PermissionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchTemplate.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PasswordHashServiceOptions>(configuration.GetSection("PasswordHash"));
        services.AddSingleton<IPasswordHashService, PasswordHashService>();
        
        // Custom Auth Service
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        // Seeding
        services.AddScoped<IDbInitializer, DbInitializer>();
        
        return services;
    }
}