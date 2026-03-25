using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.JwtService;
using CleanArchTemplate.Application.Services.Auth.VerificationLinkService;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService.Models;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;

namespace CleanArchTemplate.Application.Features.Auth.Commands;

public sealed record VerifyEmailCommand(string Token) : ICommand<Result<TokenOutput>>;

internal sealed class VerifyEmailCommandHandler(
    IVerificationTokenService verificationTokenService,
    IUserRepository userRepository,
    IJwtService jwtService
    ) : ICommandHandler<VerifyEmailCommand, Result<TokenOutput>>
{
    public async Task<Result<TokenOutput>> Handle(VerifyEmailCommand command, CancellationToken ct)
    {
        var tokenInfo = verificationTokenService.ParseToken(command.Token);
        if (tokenInfo is null)
            return Result<TokenOutput>.Validation("Invalid link");

        var user = await userRepository.GetAsync(tokenInfo.UserId, ct);
        if (user is null)
            return Result<TokenOutput>.Validation("Invalid link");
        
        if (!verificationTokenService.IsTokenValid(user, tokenInfo, TokenMotive.ConfirmEmail))
            return Result<TokenOutput>.Validation("Invalid link");

        await userRepository.ConfirmEmailAsync(user, ct);
        
        var token = jwtService.CreateToken(new TokenInput
        {
            User = user.Id,
            Email = user.Email,
            RoleNames = user.Roles.Select(r => r.Name).ToList()
        });
        
        return Result<TokenOutput>.Success(token);
    }
}