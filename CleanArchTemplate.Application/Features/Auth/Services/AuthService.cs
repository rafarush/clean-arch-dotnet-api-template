// Application/Features/Auth/Services/AuthService.cs

using CleanArchTemplate.Aplication.Features.Auth.Models.Input;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.Infrastructure.Services.Auth;

namespace CleanArchTemplate.Aplication.Features.Auth.Services;

public class AuthService(
    IUserRepository userRepository, 
    IJwtService jwtService, 
    IPasswordHashService passwordHashService) : IAuthService
{
    public async Task<AuthResult> SignInAsync(string email, string password, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(email, ct);
        
        if (user == null)
            return AuthResult.Failure("User not found");
        
        var passHashed = await passwordHashService.HashPassword(password);
        if (!await passwordHashService.ValidatePassword(passHashed, user.Password))
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