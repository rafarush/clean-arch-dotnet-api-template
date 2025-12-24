using Microsoft.EntityFrameworkCore;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;


namespace CleanArchTemplate.Infrastructure.Services.Auth.PermissionService;

public class PermissionService(AppDbContext dbContext) : IPermissionService
{
    public async Task<bool> HasPermissionAsync(Guid userId, string policyName, CancellationToken ct = default)    
    {               
        return await dbContext.Users
            .Where(u => u.Id == userId && !u.IsDeleted)           
            .SelectMany(u => u.Roles)            
            .SelectMany(r => r.Policies)            
            .AnyAsync(p => p.Name == policyName, ct);    
    }
}