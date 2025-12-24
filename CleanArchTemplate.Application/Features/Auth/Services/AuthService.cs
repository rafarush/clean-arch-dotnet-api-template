// Application/Features/Auth/Services/AuthService.cs

using CleanArchTemplate.Aplication.Features.Auth.Models.Input;
using CleanArchTemplate.Infrastructure.Repositories.User;

namespace CleanArchTemplate.Aplication.Features.Auth.Services;

public class AuthService(IUserRepository userRepository, IJwtService jwtService) : IAuthService
{
    public async Task<AuthResult> SignInAsync(string email, string password, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(email, ct);
        
        if (user == null)
            return AuthResult.Failure("User not found");
        
        if (password != user.Password)
            return AuthResult.Failure("Wrong Password");
        
        var token = jwtService.CreateToken(new TokenInput
        {
            User = user.Id,
            Email = user.Email,
            RoleNames = user.Roles.Select(r => r.Name).ToList()
        });
        
        return AuthResult.Success(token, user);
    }
}