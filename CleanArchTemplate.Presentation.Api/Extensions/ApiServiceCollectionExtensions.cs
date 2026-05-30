using System.Security.Claims;
using System.Text;
using CleanArchTemplate.Application.Persistence;
using CleanArchTemplate.Application.Services.Auth.JwtService.Options;
using CleanArchTemplate.Application.Services.Auth.VerificationLinkService;
using CleanArchTemplate.Application.Services.Auth.VerificationLinkService.Options;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService;
using CleanArchTemplate.Application.Services.Email;
using CleanArchTemplate.Application.Services.Email.Abstractions;
using CleanArchTemplate.Application.Services.Email.Options;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration config, ILogger logger)
    {
        services.Configure<VerificationTokenOptions>(config.GetSection(VerificationTokenOptions.Section));
        services.AddScoped<IVerificationTokenService, VerificationTokenService>();
        
        services.Configure<JwtOptions>(config.GetSection(JwtOptions.Section));
        services.AddAuthentication(x =>
        {
            x.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddJwtBearer(x =>
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
        }).AddGoogle("Google", options =>
        {
            options.ClientId = config["OAuth:Google:ClientId"] ?? "";
            options.ClientSecret = config["OAuth:Google:ClientSecret"] ?? "";
            options.CallbackPath = "/signin-google";
            options.Events.OnTicketReceived = context =>
            {
                LogClaims(context.Principal, "Google", logger);
                
                var email = GetClaim(context.Principal, ClaimTypes.Email, "email");
                var name = GetClaim(context.Principal, ClaimTypes.Name, "name", "given_name");
                var providerId = GetClaim(context.Principal, ClaimTypes.NameIdentifier, "sub", "urn:oid:0.9.2342.19200300.100.1.1");
                
                logger.LogInformation("Google OAuth - email: {Email}, name: {Name}, providerId: {ProviderId}", email, name, providerId);
                
                if (string.IsNullOrEmpty(email))
                {
                    context.Response.Redirect($"/api/auth/oauth/email-required?provider=Google&name={Uri.EscapeDataString(name ?? "")}&providerId={Uri.EscapeDataString(providerId ?? "")}");
                    return Task.CompletedTask;
                }
                
                context.Response.Redirect($"/api/auth/oauth/callback?provider=Google&email={Uri.EscapeDataString(email)}&name={Uri.EscapeDataString(name ?? "")}&providerId={Uri.EscapeDataString(providerId ?? "")}");
                return Task.CompletedTask;
            };
            options.Events.OnAccessDenied = context =>
            {
                logger.LogWarning("Google OAuth access denied");
                context.Response.Redirect("/api/auth/oauth/callback?error=access_denied");
                return Task.CompletedTask;
            };
        }).AddGitHub("GitHub", options =>
        {
            options.ClientId = config["OAuth:GitHub:ClientId"] ?? "";
            options.ClientSecret = config["OAuth:GitHub:ClientSecret"] ?? "";
            options.CallbackPath = "/signin-github";
            options.Events.OnTicketReceived = context =>
            {
                LogClaims(context.Principal, "GitHub", logger);
                
                var email = GetClaim(context.Principal, ClaimTypes.Email, "email")
                    ?? GetClaim(context.Principal, ClaimTypes.Name, "preferred_username");
                var name = GetClaim(context.Principal, ClaimTypes.Name, "name", "preferred_username");
                var providerId = GetClaim(context.Principal, ClaimTypes.NameIdentifier, "urn:oid:0.9.2342.19200300.100.1.1", "sub");
                
                logger.LogInformation("GitHub OAuth - email: {Email}, name: {Name}, providerId: {ProviderId}", email, name, providerId);
                
                if (string.IsNullOrEmpty(email))
                {
                    context.Response.Redirect($"/api/auth/oauth/email-required?provider=GitHub&name={Uri.EscapeDataString(name ?? "")}&providerId={Uri.EscapeDataString(providerId ?? "")}");
                    return Task.CompletedTask;
                }
                
                context.Response.Redirect($"/api/auth/oauth/callback?provider=GitHub&email={Uri.EscapeDataString(email)}&name={Uri.EscapeDataString(name ?? "")}&providerId={Uri.EscapeDataString(providerId ?? "")}");
                return Task.CompletedTask;
            };
            options.Events.OnAccessDenied = context =>
            {
                logger.LogWarning("GitHub OAuth access denied");
                context.Response.Redirect("/api/auth/oauth/callback?error=access_denied");
                return Task.CompletedTask;
            };
        });

        services.AddHttpClient();
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
    
    private static void LogClaims(ClaimsPrincipal? principal, string provider, ILogger logger)
    {
        if (principal == null)
        {
            logger.LogWarning("{Provider} OAuth: No principal found", provider);
            return;
        }
        
        var claims = principal.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
        logger.LogInformation("{Provider} OAuth claims: {@Claims}", provider, claims);
    }
    
    private static string? GetClaim(ClaimsPrincipal? principal, params string[] claimTypes)
    {
        if (principal == null) return null;
        
        foreach (var claimType in claimTypes)
        {
            var value = principal.FindFirst(claimType)?.Value;
            if (!string.IsNullOrEmpty(value))
                return value;
        }
        
        return null;
    }
}