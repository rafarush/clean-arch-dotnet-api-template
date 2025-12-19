using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.Infrastructure.Repositories.User;
using Domain.Users;

public class UserRepository(AppDbContext db) :  IUserRepository
{
    public async Task<Guid> CreateAsync(User user, CancellationToken ct)
    {
        await db.Set<User>().AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
        return await Task.FromResult(user.Id);
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken ct)
    {
        var userToUpdate = await db.Set<User>()
            .Where(x => x.Id == user.Id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        if (userToUpdate is null)
            await Task.FromResult(false);
        
        userToUpdate!.Name = user.Name;
        userToUpdate.LastName = user.LastName;
        userToUpdate.UpdatedAt = DateTime.UtcNow;
        
        
        await db.SaveChangesAsync(ct);
        
        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var user = await db.Set<User>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        if (user is null)
            return await Task.FromResult(false);
        
        user.IsDeleted = true;
        
        await db.SaveChangesAsync(ct);
        
        return await Task.FromResult(true);
    }

    public async Task<User?> GetAsync(Guid id, CancellationToken ct)
    {
        var user = await db.Set<User>()
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
            
        return await Task.FromResult(user);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct)
    {
        var users = await db.Set<User>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);
        
        return await Task.FromResult(users);
    }
}