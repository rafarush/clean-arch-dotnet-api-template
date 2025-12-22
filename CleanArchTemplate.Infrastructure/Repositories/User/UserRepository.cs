using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using CleanArchTemplate.SharedKernel.Models.General.Output;
using CleanArchTemplate.SharedKernel.Models.Input.User.Models.Output;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using CleanArchTemplate.SharedKernel.Models.User.Params;
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
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(ct);
            
        return await Task.FromResult(user);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var user = await db.Set<User>()
            .AsNoTracking()
            .Where(x => x.Email == email && !x.IsDeleted)
            .Include(x => x.Roles)
                .ThenInclude(r => r.Policies)
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

    public async Task<PaginatedOutput<UserOutput>> SearchUsersAsync(SearchUsersParams usersParams, CancellationToken ct)
    {
        var users = db.Set<User>()
            .AsNoTracking()
            .AsQueryable()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(usersParams.Email))
            users = users.Where(x => x.Email == usersParams.Email);
        
        if (!string.IsNullOrWhiteSpace(usersParams.Name))
            users = users.Where(x=> x.Name ==  usersParams.Name);
        
        if (!string.IsNullOrWhiteSpace(usersParams.LastName))
            users = users.Where(x => x.LastName == usersParams.LastName);
        
        if (usersParams.CreatedAt.HasValue)
            users = users.Where(x => x.CreatedAt >= usersParams.CreatedAt.Value);
        
        if (usersParams.UpdatedAt.HasValue)
            users = users.Where(x => x.UpdatedAt >= usersParams.UpdatedAt.Value);
        
        var count = await users.CountAsync(ct);
        
        if (count == 0)
            return await Task.FromResult(new PaginatedOutput<UserOutput>([], 0));
        
        // Sorting
        users = ApplyOrdering(users, usersParams.OffsetField, usersParams.IsAsc);
        
        if (usersParams.HasPagination)
        {
            users = users
                .Skip((usersParams.OffsetPage - 1) * usersParams.Limit)
                .Take(usersParams.Limit);
        }

        var rows = await users
            .Select(x => new UserOutput
            {
                Id = x.Id,
                Name = x.Name,
                LastName = x.LastName,
                Email = x.Email,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                IsDeleted = x.IsDeleted })
            .ToListAsync(ct);
        
        return await Task.FromResult(new PaginatedOutput<UserOutput>(rows, count));
    }
    
    private IQueryable<User> ApplyOrdering(
        IQueryable<User> query, 
        string orderBy, 
        bool ascending)
    {
        return orderBy.ToLower() switch
        {
            "name" => ascending ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name),
            "lastname" => ascending ? query.OrderBy(u => u.LastName) : query.OrderByDescending(u => u.LastName),
            "email" => ascending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            "created_at" => ascending ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt),
            "updated_at" => ascending ? query.OrderBy(u => u.UpdatedAt) : query.OrderByDescending(u => u.UpdatedAt),
            "id" => ascending ? query.OrderBy(u => u.Id) : query.OrderByDescending(u => u.Id),
            _ => query.OrderByDescending(u => u.CreatedAt)
        };
    }
}