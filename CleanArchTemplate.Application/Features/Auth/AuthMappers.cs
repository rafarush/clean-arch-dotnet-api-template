using CleanArchTemplate.SharedKernel.Models.Auth.Input;

namespace CleanArchTemplate.Application.Features.Auth;

public static class AuthMappers
{
    public static Domain.Users.User ToUser(this SignUpInput input, string passHashed)
    {
        return new Domain.Users.User
        {
            Name = input.Name,
            LastName = input.LastName,
            Email = input.Email,
            Password = passHashed,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
        };
    }
}