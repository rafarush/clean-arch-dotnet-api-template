using CleanArchTemplate.Application.Middleware;
using CleanArchTemplate.Application.Extensions;
using CleanArchTemplate.Infrastructure.Extensions;



var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add API Services
builder.Services.AddApiServices(config);

// Add Infrastructure Services
builder.Services.AddInfrastructure(config);

// Add Application Services
builder.Services.AddApplication(config);

// Add Db
builder.Services.AddDatabase(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidatorMapperMiddleware>();

app.MapControllers();

await app.InitializeDatabaseAsync();

app.Run();


