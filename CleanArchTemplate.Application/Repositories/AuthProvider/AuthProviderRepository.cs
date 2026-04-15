using AuthProviderEntity = CleanArchTemplate.Domain.AuthProvider.AuthProvider;
using CleanArchTemplate.Application.Repositories.AuthProvider;
using CleanArchTemplate.Domain.AuthProvider;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.Application.Repositories.AuthProvider;

public class AuthProviderRepository(AppDbContext db) : IAuthProviderRepository
{
    public async Task<AuthProviderEntity?> GetByProviderAsync(OAuthProviderType provider, string providerUserId, CancellationToken ct)
    {
        return await db.Set<AuthProviderEntity>()
            .Include(x => x.User)
            .ThenInclude(u => u.Roles)
            .ThenInclude(r => r.Policies)
            .FirstOrDefaultAsync(x => x.Provider == provider && x.ProviderUserId == providerUserId, ct);
    }

    public async Task<AuthProviderEntity?> GetByUserAndProviderAsync(Guid userId, OAuthProviderType provider, CancellationToken ct)
    {
        return await db.Set<AuthProviderEntity>()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Provider == provider, ct);
    }

    public async Task<Guid> CreateAsync(AuthProviderEntity authProvider, CancellationToken ct)
    {
        await db.Set<AuthProviderEntity>().AddAsync(authProvider, ct);
        await db.SaveChangesAsync(ct);
        return authProvider.Id;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.Set<AuthProviderEntity>()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        
        if (entity is null)
            return false;
        
        db.Set<AuthProviderEntity>().Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteByUserAndProviderAsync(Guid userId, OAuthProviderType provider, CancellationToken ct)
    {
        var entity = await db.Set<AuthProviderEntity>()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Provider == provider, ct);
        
        if (entity is null)
            return false;
        
        db.Set<AuthProviderEntity>().Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
