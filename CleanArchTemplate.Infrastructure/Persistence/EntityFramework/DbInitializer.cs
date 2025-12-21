using CleanArchTemplate.Domain.Security;
using CleanArchTemplate.Domain.Users;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchTemplate.Infrastructure.Persistence.EntityFramework;

public interface IDbInitializer
{
    Task InitializeAsync();
    Task SeedAsync();
}

public class DbInitializer(
    AppDbContext db, 
    ILogger<DbInitializer> logger, 
    IServiceScopeFactory serviceScopeFactory) : IDbInitializer
{
    
    public async Task InitializeAsync()
    {
        try
        {
            logger.LogInformation("Initializing database...");

            // Apply pending migrations
            await db.Database.MigrateAsync();
            
            logger.LogInformation("Migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something went wrong while initializing the database.");
            throw;
        }
    }

    // Seeders
    public async Task SeedAsync()
    {
        try
        {
            logger.LogInformation("Starting seeding database...");

            // Policies
            if (!db.Policies.Any())
            {
                await SeedPoliciesAsync();
            }
            
            // Roles
            if (!db.Roles.Any())
            {
                await SeedRolesAsync();
            }
            
            // Users
            if (!db.Users.Any())
            {
                await SeedUsersAsync();
            }
            
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
        logger.LogInformation("Seeding users...");
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Add Users
        UserSeeder seeder = new UserSeeder(dbContext);
        var users =  new List<User>(seeder.GetUsers());
        
        foreach (var user in users)
        {
            if (!dbContext.Users.Any(u => u.Id == user.Id || u.Email == user.Email))
            {
                await dbContext.Users.AddAsync(user);
            }
        }
        
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Initial users added. ({Count} users)", users.Count);
    }

    private async Task SeedPoliciesAsync()
    {
        logger.LogInformation("Seeding policies...");
        // Add Policies
        List<Policy> policies = [];
        
        // Policies policies XD
        policies.AddRange(PolicySeeder.GetPolicyPolicies());
        // Users policies
        policies.AddRange(PolicySeeder.GetUserPolicies());
        // Roles policies
        policies.AddRange(PolicySeeder.GetRolePolicies());

        foreach (var policy in policies)
        {
            if (!db.Policies.Any(p => p.Id == policy.Id || p.Name == policy.Name))
            {
                await db.Policies.AddAsync(policy);
            }
        }
        
        // await db.SaveChangesAsync();
        logger.LogInformation("Initial policies added. ({Count} policies)",  policies.Count);
    }

    private async Task SeedRolesAsync()
    {
        logger.LogInformation("Seeding roles...");
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Add roles
        RoleSeeder seeder = new RoleSeeder(dbContext);
        List<Role> roles = seeder.GetRoles();
        
        foreach (var role in roles)
        {
            if (!dbContext.Roles.Any(r => r.Id == role.Id || r.Name == role.Name))
            {
                await dbContext.Roles.AddAsync(role);
            }
        }
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Initial roles added. ({Count} roles)",  roles.Count);
    }

    
}