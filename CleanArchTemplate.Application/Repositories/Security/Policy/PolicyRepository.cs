using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.Application.Repositories.Security.Policy;

public class PolicyRepository(AppDbContext db) : IPolicyRepository
{
    public async Task<Guid> CreateAsync(Domain.Security.Policy policy, CancellationToken ct)
    {
        await db.Set<Domain.Security.Policy>().AddAsync(policy, ct);
        await db.SaveChangesAsync(ct);
        return policy.Id;
    }

    public async Task<bool> UpdateAsync(Domain.Security.Policy policy, CancellationToken ct)
    {
        try
        {
            db.Entry(policy).State = EntityState.Modified;
            
            db.Entry(policy).Property(x => x.Id).IsModified = false;
            db.Entry(policy).Property(x => x.CreatedAt).IsModified = false;
            
            await db.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var policy = await db.Set<Domain.Security.Policy>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        if (policy is null)
            return false;
        
        policy.IsDeleted = true;
        
        await db.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<Domain.Security.Policy?> GetAsync(Guid id, CancellationToken ct)
    {
        var policy = await db.Set<Domain.Security.Policy>()
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        return policy;
    }

    public async Task<IEnumerable<Domain.Security.Policy>> GetAllAsync(CancellationToken ct)
    {
        var policies = await db.Set<Domain.Security.Policy>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);
        
        return policies;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        => await db.Set<Domain.Security.Policy>().AsNoTracking().AnyAsync(x => x.Id == id && !x.IsDeleted, ct);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
        => await db.Set<Domain.Security.Policy>().AsNoTracking().AnyAsync(x => x.Name == name && !x.IsDeleted, ct);

    public async Task<List<Domain.Security.Policy>> GetByIdsAsync(List<Guid> ids, CancellationToken ct)
        => await db.Set<Domain.Security.Policy>()
                .Where(x => !x.IsDeleted && ids.Contains(x.Id))
                .ToListAsync(ct);
    
}