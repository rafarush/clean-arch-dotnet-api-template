using CleanArchTemplate.Domain.Security;
using CleanArchTemplate.Infrastructure.Persistence.EntityFramework;
using CleanArchTemplate.SharedKernel.Models.General.Output;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using Microsoft.EntityFrameworkCore;

namespace CleanArchTemplate.Application.Repositories.User;

public class UserRepository(AppDbContext db) :  IUserRepository
{
    public async Task<Guid> CreateAsync(Domain.User.User user, CancellationToken ct, List<Role>? roles = null)
    {
        if (roles is { Count: > 0 })
        {
            foreach (var role in roles.Where(r => !user.Roles.Contains(r)))
                user.Roles.Add(role);
        }
        await db.Set<Domain.User.User>().AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task<bool> UpdateAsync(Domain.User.User user, CancellationToken ct)
    {
        var userToUpdate = await db.Set<Domain.User.User>()
            .Where(x => x.Id == user.Id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        if (userToUpdate is null)
            return false;
        
        userToUpdate!.Name = user.Name;
        userToUpdate.LastName = user.LastName;
        
        await db.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var user = await db.Set<Domain.User.User>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
        
        if (user is null)
            return false;
        
        user.IsDeleted = true;
        
        await db.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<Domain.User.User?> GetAsync(Guid id, CancellationToken ct)
    {
        var user = await db.Set<Domain.User.User>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
            
        return user;
    }
    
    public async Task<Domain.User.User?> GetWithRelationsAsync(Guid id, CancellationToken ct)
    {
        var user = await db.Set<Domain.User.User>()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Include(x => x.Roles)
            .ThenInclude(x => x.Policies)
            .FirstOrDefaultAsync(ct);
            
        return user;
    }

    public async Task<Domain.User.User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var user = await db.Set<Domain.User.User>()
            .AsNoTracking()
            .Where(x => x.Email == email)
            .Include(x => x.Roles)
                .ThenInclude(r => r.Policies)
            .FirstOrDefaultAsync(ct);
            
        return user;
    }

    public async Task<IEnumerable<Domain.User.User>> GetAllAsync(CancellationToken ct)
    {
        var users = await db.Set<Domain.User.User>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);
        
        return users;
    }
    
    public async Task<Domain.User.User> AssignRolesToUserAsync(Domain.User.User user, List<Role> roles, CancellationToken ct)
    {
        foreach (var r in roles.Where(r => !user.Roles.Contains(r)))
            user.Roles.Add(r);
        await db.SaveChangesAsync(ct);
        return user;
    }

    public async Task ConfirmEmailAsync(Domain.User.User user, CancellationToken ct)
    {
        user.EmailVerified = true;
        user.ConfirmationCode = null;
        await db.SaveChangesAsync(ct);
    }

    public async Task<PaginatedOutput<UserOutput>> SearchUsersAsync(SearchUsersInput usersInput, CancellationToken ct)
    {
        var users = db.Set<Domain.User.User>()
            .AsNoTracking()
            .AsQueryable()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(usersInput.Email))
            users = users.Where(x => x.Email == usersInput.Email);
        
        if (!string.IsNullOrWhiteSpace(usersInput.Name))
            users = users.Where(x=> x.Name ==  usersInput.Name);
        
        if (!string.IsNullOrWhiteSpace(usersInput.LastName))
            users = users.Where(x => x.LastName == usersInput.LastName);
        
        if (usersInput.CreatedAt.HasValue)
            users = users.Where(x => x.CreatedAt >= usersInput.CreatedAt.Value);
        
        if (usersInput.UpdatedAt.HasValue)
            users = users.Where(x => x.UpdatedAt >= usersInput.UpdatedAt.Value);
        
        var count = await users.CountAsync(ct);
        
        if (count == 0)
            return new PaginatedOutput<UserOutput>([], 0);
        
        // Sorting
        users = ApplyOrdering(users, usersInput.OffsetField, usersInput.IsAsc);
        
        if (usersInput.HasPagination)
        {
            users = users
                .Skip((usersInput.OffsetPage - 1) * usersInput.Limit)
                .Take(usersInput.Limit);
        }

        var rows = await users
            .Select(x => new UserOutput
            {
                Id = x.Id,
                Name = x.Name,
                LastName = x.LastName,
                Email = x.Email,
                CreatedAt = x.CreatedAt.DateTime,
                UpdatedAt = x.UpdatedAt.DateTime,
                IsDeleted = x.IsDeleted })
            .ToListAsync(ct);

        return new PaginatedOutput<UserOutput>(rows, count);
    }
    
    private IQueryable<Domain.User.User> ApplyOrdering(
        IQueryable<Domain.User.User> query, 
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