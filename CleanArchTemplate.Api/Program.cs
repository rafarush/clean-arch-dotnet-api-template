using CleanArchTemplate.Aplication.Extensions;
using CleanArchTemplate.Aplication.Middleware;
using CleanArchTemplate.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ValidatorMapperMiddleware>();

app.MapControllers();

await app.InitializeDatabaseAsync();

app.Run();


