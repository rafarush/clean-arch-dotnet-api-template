using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using CleanArchTemplate.Infrastructure.Repositories.Security.Policy;
using CleanArchTemplate.Infrastructure.Repositories.Security.Role;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.Infrastructure.Services.Auth.PermissionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchTemplate.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        
        
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