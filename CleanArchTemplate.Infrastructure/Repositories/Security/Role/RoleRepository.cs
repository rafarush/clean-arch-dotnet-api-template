using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.Infrastructure.Repositories.Security.Role;

using Domain.Security;

public class RoleRepository(AppDbContext db) :  IRoleRepository
{
    public async Task<Guid> CreateAsync(Role role, CancellationToken ct)
    {
        await db.Set<Role>().AddAsync(role, ct);
        await db.SaveChangesAsync(ct);
        return await Task.FromResult(role.Id);
    }

    public async Task<bool> UpdateAsync(Role role, CancellationToken ct)
    {
        var roleToUpdate = await db.Set<Role>()
            .Where(x => x.Id ==  role.Id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        if ( roleToUpdate is null)
            await Task.FromResult(false);
        
        roleToUpdate!.Name =  role.Name;
        roleToUpdate.Description = role.Description;
        roleToUpdate.Policies = role.Policies;
        
        await db.SaveChangesAsync(ct);
        
        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var role = await db.Set<Role>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        if (role is null)
            return await Task.FromResult(false);
        
        role.IsDeleted = true;
        
        await db.SaveChangesAsync(ct);
        
        return await Task.FromResult(true);
    }

    public async Task<Role?> GetAsync(Guid id, CancellationToken ct)
    {
        var role = await db.Set<Role>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Include(x=>x.Policies)
            .FirstOrDefaultAsync(ct);
            
        return await Task.FromResult(role);
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken ct)
    {
        var roles = await db.Set<Role>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);
        
        return await Task.FromResult(roles);
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken ct)
    {
        return await db.Set<Role>().AsNoTracking().AnyAsync(x => x.Name == name && !x.IsDeleted, ct);
    }

    public async Task<Role> AssignPoliciesToRoleAsync(Role role, List<Policy> policies, CancellationToken ct)
    {
        foreach (var p in policies.Where(p => !role.Policies.Contains(p)))
            role.Policies.Add(p);
        await db.SaveChangesAsync(ct);
        return await Task.FromResult(role);
    }

    public async Task<List<Role>> GetByIdsAsync(List<Guid> ids, CancellationToken ct)
        => await db.Set<Role>()
            .Where(x => !x.IsDeleted && ids.Contains(x.Id))
            .ToListAsync(ct);
}