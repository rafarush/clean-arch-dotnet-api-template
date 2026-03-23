using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.Application.Repositories.Security.Role;

public class RoleRepository(AppDbContext db) :  IRoleRepository
{
    public async Task<Guid> CreateAsync(Domain.Security.Role role, CancellationToken ct)
    {
        await db.Set<Domain.Security.Role>().AddAsync(role, ct);
        await db.SaveChangesAsync(ct);
        return role.Id;
    }

    public async Task<bool> UpdateAsync(Domain.Security.Role role, CancellationToken ct)
    {
        var roleToUpdate = await db.Set<Domain.Security.Role>()
            .Where(x => x.Id ==  role.Id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        if (roleToUpdate is null)
            return false;
        
        roleToUpdate.Name =  role.Name;
        roleToUpdate.Description = role.Description;
        roleToUpdate.Policies = role.Policies;
        
        await db.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var role = await db.Set<Domain.Security.Role>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        if (role is null)
            return false;
        
        role.IsDeleted = true;
        
        await db.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<Domain.Security.Role?> GetAsync(Guid id, CancellationToken ct)
    {
        var role = await db.Set<Domain.Security.Role>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Include(x=>x.Policies)
            .FirstOrDefaultAsync(ct);
            
        return role;
    }

    public async Task<Domain.Security.Role?> GetByNameAsync(string name, CancellationToken ct)
    {
        var role = await db.Set<Domain.Security.Role>()
            .Where(x => x.Name == name && !x.IsDeleted)
            .Include(x=>x.Policies)
            .FirstOrDefaultAsync(ct);
            
        return role;
    }

    public async Task<IEnumerable<Domain.Security.Role>> GetAllAsync(CancellationToken ct)
    {
        var roles = await db.Set<Domain.Security.Role>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);
        
        return roles;
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken ct)
    {
        return await db.Set<Domain.Security.Role>().AsNoTracking().AnyAsync(x => x.Name == name && !x.IsDeleted, ct);
    }

    public async Task<Domain.Security.Role> AssignPoliciesToRoleAsync(Domain.Security.Role role, List<Domain.Security.Policy> policies, CancellationToken ct)
    {
        foreach (var p in policies.Where(p => !role.Policies.Contains(p)))
            role.Policies.Add(p);
        await db.SaveChangesAsync(ct);
        return role;
    }

    public async Task<List<Domain.Security.Role>> GetByIdsAsync(List<Guid> ids, CancellationToken ct)
        => await db.Set<Domain.Security.Role>()
            .Where(x => !x.IsDeleted && ids.Contains(x.Id))
            .ToListAsync(ct);
}