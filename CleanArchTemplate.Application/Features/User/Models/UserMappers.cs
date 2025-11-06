using CleanArchTemplate.Aplication.Features.User.Models.Output;

namespace CleanArchTemplate.Aplication.Features.User.Models;

using CleanArchTemplate.Aplication.Features.User.Models.Input;
using Domain.Users;
public static class UserMappers
{
    public static User ToUser(this CreateUserInput input)
    {
        return new User
        {
            Name = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Password = input.Password,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
        };
    }
    
    public static User ToUser(this UpdateUserInput input, Guid id)
    {
        return new User
        {
            Name = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Password = input.Password,
            Id = id,
            CreatedAt = DateTime.Now,
        };
    }

    public static UserOutput ToOutput(this User user)
    {
        return new UserOutput
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.Name,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted
        };
    }
}