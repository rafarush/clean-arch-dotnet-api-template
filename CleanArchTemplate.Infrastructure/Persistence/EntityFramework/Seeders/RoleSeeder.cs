using CleanArchTemplate.Domain.Security;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.Infrastructure.Persistence.EntityFramework.Seeders;

public class RoleSeeder(AppDbContext db)
{

    public List<Role> GetRoles()
    {
        return
        [
            new Role
            {
                Name = "Admin",
                Description = "Total Admin",
                Policies = GetAdminPolicies()
            },
            
            new Role
            {
                Name = "User",
                Description = "Regular User",
                Policies = GetUserPolicies()
            }

        ];
    }

    private List<Policy> GetAdminPolicies()
    {
        var policies = db.Set<Policy>().ToList();
        return policies;
    }

    private List<Policy> GetUserPolicies()
    {
        List<Policy> policies = [];
        var viewMePolicy = db.Set<Policy>().FirstOrDefault(x => x.Name == "view-me");
        policies.Add(viewMePolicy ?? throw new InvalidOperationException("view-me policy not found"));
        
        
        return policies;
    }
    
}