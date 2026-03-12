using CleanArchTemplate.Application.Features.Auth.Services;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;

namespace CleanArchTemplate.Application.Features.User;

using Domain.Users;

public static class UserMappers
{
    public static User ToUser(this CreateUserInput input, byte[] passHashed)
    {
        return new User
        {
            Name = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Password = passHashed,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
        };
    }
    
    public static User ToUserUpdate(this UpdateUserInput input, User userOld)
    {
        return new User
        {
            Name = input.FirstName,
            LastName = input.LastName,
            Email = userOld.Email,
            Password = userOld.Password,
            Id = userOld.Id
        };
    }

    public static UserOutput ToOutput(this User user)
    {
        return new UserOutput
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted
        };
    }
}