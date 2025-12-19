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
            return AuthResult.Failure("Usuario no encontrado");
        
        // NOTA: Esto es solo para desarrollo. En producción usa hashing
        if (password != user.Password)
            return AuthResult.Failure("Contraseña incorrecta");
        
        var token = jwtService.CreateToken(new TokenInput
        {
            Email = user.Email,
            Policies = user.Roles.SelectMany(static x => x.Policies)
                .ToList(),
            User = user.Id,
            Roles = user.Roles
        });
        
        return AuthResult.Success(token, user);
    }
}