using CleanArchTemplate.Domain.Security;
using CleanArchTemplate.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.Infrastructure.Persistence.EntityFramework.Seeders;

public class UserSeeder(AppDbContext db)
{
    public List<User> GetUsers()
    {
        return
        [
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                Name = "Administrator",
                LastName = "Administrator",
                Password = "admin",
                Roles = GetAdminRoles(),
                CreatedAt = DateTime.UtcNow
            },
            new User 
            { 
                Id = Guid.NewGuid(), 
                Email = "user@example.com", 
                Name = "User",
                LastName = "Regular",
                Password = "user",
                Roles = GetRegularUserRoles(),
                CreatedAt = DateTime.UtcNow
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