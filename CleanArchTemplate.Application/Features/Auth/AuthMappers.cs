using CleanArchTemplate.SharedKernel.Models.Auth.Input;

namespace CleanArchTemplate.Application.Features.Auth;

public static class AuthMappers
{
    public static Domain.User.User ToUser(this SignUpInput input, string passHashed)
    {
        return new Domain.User.User
        {
            Name = input.Name,
            LastName = input.LastName,
            Email = input.Email,
            Password = passHashed,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            EmailVerified = false
        };
    }
}