using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.Infrastructure.Repositories.Security.Policy;

using Domain.Security;

public class PolicyRepository(AppDbContext db) : IPolicyRepository
{
    public async Task<Guid> CreateAsync(Policy policy, CancellationToken ct)
    {
        await db.Set<Policy>().AddAsync(policy, ct);
        await db.SaveChangesAsync(ct);
        return await Task.FromResult(policy.Id);
    }

    public async Task<bool> UpdateAsync(Policy policy, CancellationToken ct)
    {
        try
        {
            db.Entry(policy).State = EntityState.Modified;
            
            db.Entry(policy).Property(x => x.Id).IsModified = false;
            db.Entry(policy).Property(x => x.CreatedAt).IsModified = false;
            
            await db.SaveChangesAsync(ct);
            return await Task.FromResult(true);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var policy = await db.Set<Policy>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        if (policy is null)
            await Task.FromResult(false);
        
        policy!.IsDeleted = true;
        
        await db.SaveChangesAsync(ct);
        
        return await Task.FromResult(true);
    }

    public async Task<Policy?> GetAsync(Guid id, CancellationToken ct)
    {
        var policy = await db.Set<Policy>()
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        return await Task.FromResult(policy);
    }

    public async Task<IEnumerable<Policy>> GetAllAsync(CancellationToken ct)
    {
        var policies = await db.Set<Policy>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);
        
        return await Task.FromResult(policies);
    }
}