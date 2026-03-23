using System.Text;
using CleanArchTemplate.Application.Features.Auth.Options;
using CleanArchTemplate.Application.Persistence;
using CleanArchTemplate.Application.Services.Email;
using CleanArchTemplate.Application.Services.Email.Abstractions;
using CleanArchTemplate.Application.Services.Email.Options;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CleanArchTemplate.Api.Extensions;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        
        // Seeding
        services.AddScoped<IDbInitializer, DbInitializer>();
        
        return services;
    }
    
    public static IServiceCollection AddEmailService(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SmtpOptions>(config.GetSection(SmtpOptions.Section));
        services.Configure<ScribanOptions>(config.GetSection(ScribanOptions.Section));

        services.AddScoped<ITemplateRenderer, ScribanTemplateRenderer>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(config.GetSection(JwtOptions.Section));

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