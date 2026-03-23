using System.IdentityModel.Tokens.Jwt;
using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Features.Auth.Services;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Domain.Security;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;

namespace CleanArchTemplate.Application.Features.Auth.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<Result<TokenOutput>>;


internal sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IJwtService jwtService
    ) : ICommandHandler<RefreshTokenCommand, Result<TokenOutput>>
{
    public async Task<Result<TokenOutput>> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(command.RefreshToken))
            return Result<TokenOutput>.Unauthorized("Invalid refresh token");

        var jwtToken = handler.ReadJwtToken(command.RefreshToken);

        if (jwtToken.ValidTo < DateTime.UtcNow)
            return Result<TokenOutput>.Unauthorized("Refresh token has expired");

        var email = jwtToken.Claims
            .FirstOrDefault(c => c.Type == AppClaims.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return Result<TokenOutput>.Unauthorized("Invalid refresh token");

        var user = await userRepository.GetByEmailAsync(email, ct);

        if (user == null)
            return Result<TokenOutput>.Unauthorized("Invalid refresh token");

        var token = jwtService.CreateToken(new TokenInput
        {
            User = user.Id,
            Email = user.Email,
            RoleNames = user.Roles.Select(r => r.Name).ToList()
        });

        return Result<TokenOutput>.Success(token);
    }
    
}