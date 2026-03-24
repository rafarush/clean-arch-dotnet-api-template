using CleanArchTemplate.Application.Services.Auth.PasswordHashService;
using CleanArchTemplate.Domain.Security;
using CleanArchTemplate.Domain.User;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;

namespace CleanArchTemplate.Application.Persistence.Seeders;

public class UserSeeder(AppDbContext db, IPasswordHashService passwordHashService)
{
    public async Task<List<User>> GetUsers()
    {
        return
        [
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                Name = "Administrator",
                LastName = "Administrator",
                Password = await passwordHashService.HashPassword("admin"),
                EmailVerified = true,
                Roles = GetAdminRoles()
            },
            new User 
            { 
                Id = Guid.NewGuid(), 
                Email = "user@example.com", 
                Name = "User",
                LastName = "Regular",
                Password = await passwordHashService.HashPassword("user"),
                EmailVerified = true,
                Roles = GetRegularUserRoles()
            }

        ];
    }

    private List<Role> GetAdminRoles()
    {
        List<Role> roles = [];
        var adminRole = db.Set<Role>().FirstOrDefault(x=> x.Name == "Admin");
        roles.Add(adminRole ?? throw new InvalidOperationException("Admin role not found"));
        
        return roles;
    }

    private List<Role> GetRegularUserRoles()
    {
        List<Role> roles = [];
        var regularRole = db.Set<Role>().FirstOrDefault(x => x.Name == "User");
        roles.Add(regularRole ?? throw new InvalidOperationException("User role not found"));
        
        return roles;
    }
}