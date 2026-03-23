using System.Reflection;
using CleanArchTemplate.Application.Features.Auth.Services;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CleanArchTemplate.Application.Features.Auth.Options;
using System.Text;
using CleanArchTemplate.Application.Persistence;
using CleanArchTemplate.Application.Repositories.Security.Policy;
using CleanArchTemplate.Application.Repositories.Security.Role;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.Options;
using CleanArchTemplate.Application.Services.Auth.PasswordHashService;
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
        services.Configure<PasswordHashServiceOptions>(config.GetSection("PasswordHash"));
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

    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(config.GetSection("Jwt"));

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!)),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = config["Jwt:Audience"],
            };
        });

        services.AddAuthorization();
        services.AddControllers();

        services.AddSwaggerGen(static x =>
        {
            x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using Bearer scheme"
            });
    
            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
    
            x.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CleanArchTemplate API",
                Version = "v1",
                Description = "Clean Architecture Template and JWT Authentication API"
            });
    
            x.EnableAnnotations();
        });

        return services;
    }
}