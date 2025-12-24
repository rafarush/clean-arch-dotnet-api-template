using CleanArchTemplate.Aplication.Features.Auth.Services;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;

namespace CleanArchTemplate.Aplication.Features.User;

using Domain.Users;
using SharedKernel.Models.User.Input;

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
    
    public static User ToUser(this UpdateUserInput input, Guid id, byte[] passHashed)
    {
        return new User
        {
            Name = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Password = passHashed,
            Id = id
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