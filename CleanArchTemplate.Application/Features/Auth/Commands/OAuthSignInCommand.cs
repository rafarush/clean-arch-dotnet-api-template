using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Repositories.AuthProvider;
using CleanArchTemplate.Application.Repositories.Security.Role;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.JwtService;
using CleanArchTemplate.Application.Services.Auth.OAuthService;
using CleanArchTemplate.Domain.AuthProvider;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;

namespace CleanArchTemplate.Application.Features.Auth.Commands;

public sealed record OAuthSignInCommand(OAuthProviderType Provider, string Code) 
    : ICommand<Result<TokenOutput>>;

internal sealed class OAuthSignInCommandHandler(
    IOAuthService oAuthService,
    IUserRepository userRepository,
    IAuthProviderRepository authProviderRepository,
    IRoleRepository roleRepository,
    IJwtService jwtService
    ) : ICommandHandler<OAuthSignInCommand, Result<TokenOutput>>
{
    public async Task<Result<TokenOutput>> Handle(OAuthSignInCommand command, CancellationToken ct)
    {
        var userInfo = await oAuthService.GetUserInfoAsync(command.Provider, command.Code, ct);

        var existingAuthProvider = await authProviderRepository.GetByProviderAsync(command.Provider, userInfo.ProviderId, ct);
        
        if (existingAuthProvider is not null)
        {
            var token = jwtService.CreateToken(new TokenInput
            {
                User = existingAuthProvider.User.Id,
                Email = existingAuthProvider.User.Email,
                RoleNames = existingAuthProvider.User.Roles.Select(r => r.Name).ToList()
            });
            return Result<TokenOutput>.Success(token);
        }

        var existingUser = await userRepository.GetByEmailWithAuthProvidersAsync(userInfo.Email, ct);

        if (existingUser is not null)
        {
            var newAuthProvider = new AuthProvider
            {
                UserId = existingUser.Id,
                Provider = command.Provider,
                ProviderUserId = userInfo.ProviderId
            };
            await authProviderRepository.CreateAsync(newAuthProvider, ct);
            
            var token = jwtService.CreateToken(new TokenInput
            {
                User = existingUser.Id,
                Email = existingUser.Email,
                RoleNames = existingUser.Roles.Select(r => r.Name).ToList()
            });
            return Result<TokenOutput>.Success(token);
        }

        var defaultRole = await roleRepository.GetByNameAsync("User", ct);
        if (defaultRole is null)
            return Result<TokenOutput>.InternalError("Default role not found");

        var nameParts = userInfo.Name.Split(' ', 2);
        var newUser = new Domain.User.User
        {
            Name = nameParts[0],
            LastName = nameParts.Length > 1 ? nameParts[1] : "",
            Email = userInfo.Email,
            EmailVerified = true,
            Password = null,
            Roles = [defaultRole]
        };

        var userId = await userRepository.CreateAsync(newUser, ct, [defaultRole]);

        var authProvider = new AuthProvider
        {
            UserId = userId,
            Provider = command.Provider,
            ProviderUserId = userInfo.ProviderId
        };
        await authProviderRepository.CreateAsync(authProvider, ct);

        var jwtToken = jwtService.CreateToken(new TokenInput
        {
            User = userId,
            Email = newUser.Email,
            RoleNames = [defaultRole.Name]
        });

        return Result<TokenOutput>.Success(jwtToken);
    }
}
