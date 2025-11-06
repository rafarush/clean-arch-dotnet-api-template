using CleanArchTemplate.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchTemplate.Infrastructure.Persistence.EntityFramework;

public interface IDbInitializer
{
    Task InitializeAsync();
    Task SeedAsync();
}

public class DbInitializer(AppDbContext db, ILogger<DbInitializer> logger) : IDbInitializer
{
    
    public async Task InitializeAsync()
    {
        try
        {
            logger.LogInformation("Initializing database...");

            // Aplicar migraciones pendientes
            await db.Database.MigrateAsync();
            
            logger.LogInformation("Migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something went wrong while initializing the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            logger.LogInformation("Starting seeding database...");

            // Verificar si ya existen datos
            if (!db.Users.Any())
            {
                await SeedUsersAsync();
            }

            // Agrega más métodos de seeding según necesites

            await db.SaveChangesAsync();
            logger.LogInformation("Seeding success.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something went wrong while seeding database.");
            throw;
        }
    }
    
    private async Task SeedUsersAsync()
    {
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                Name = "Administrator",
                CreatedAt = DateTime.UtcNow,
                LastName = "Administrator",
                Password = "admin"
            },
            new User 
            { 
                Id = Guid.NewGuid(), 
                Email = "user@example.com", 
                Name = "User",
                LastName = "Regular",
                Password = "user",
                CreatedAt = DateTime.UtcNow
            }
        };

        await db.Users.AddRangeAsync(users);
        logger.LogInformation("Initial users added. ({Count} users)", users.Count);
    }
}