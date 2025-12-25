using System.Text;
using CleanArchTemplate.Aplication.Extensions;
using CleanArchTemplate.Aplication.Features.Auth.Options;
using CleanArchTemplate.Aplication.Features.Auth.Services;
using CleanArchTemplate.Aplication.Middleware;
using CleanArchTemplate.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.Configure<JwtOptions>(config.GetSection("Jwt"));

builder.Services.AddAuthentication(x =>
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
        ValidIssuer = config["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = config["Jwt:Audience"],
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(static x =>
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
        Description = "JWT Authentication API"
    });
    
    x.EnableAnnotations();
});


// Add Infrastructure Services
builder.Services.AddInfrastructure();

// Add Application Services
builder.Services.AddApplication();

// Add Db
builder.Services.AddDatabase(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanArchTemplate API v1");
        options.RoutePrefix = "swagger";
    });
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidatorMapperMiddleware>();

app.MapControllers();

// await app.InitializeDatabaseAsync();

app.Run();


