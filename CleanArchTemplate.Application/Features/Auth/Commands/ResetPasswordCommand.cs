using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.PasswordHashService;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService.Models;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Auth.Commands;

public sealed record ResetPasswordCommand(ResetPasswordInput Input) : ICommand<Result<ResetPasswordOutput>>;

internal sealed class ResetPasswordCommandHandler(
    IVerificationTokenService verificationTokenService,
    IValidator<ResetPasswordInput> resetPasswordValidator,
    IUserRepository userRepository,
    IPasswordHashService passwordHashService
    ) : ICommandHandler<ResetPasswordCommand, Result<ResetPasswordOutput>>
{
    public async Task<Result<ResetPasswordOutput>> Handle(ResetPasswordCommand command, CancellationToken ct)
    {
        await resetPasswordValidator.ValidateAndThrowAsync(command.Input, ct);
        
        var tokenInfo = verificationTokenService.ParseToken(command.Input.Token);
        if (tokenInfo is null)
            return Result<ResetPasswordOutput>.Validation("Invalid link");

        var user = await userRepository.GetAsync(tokenInfo.UserId, ct);
        if (user is null)
            return Result<ResetPasswordOutput>.Validation("Invalid link");
        
        if (!verificationTokenService.IsTokenValid(user, tokenInfo, TokenMotive.ResetPassword))
            return Result<ResetPasswordOutput>.Validation("Invalid link");
        
        user.Password = await passwordHashService.HashPassword(command.Input.NewPassword);
        user.ResetPasswordCodes.Clear();
        await userRepository.UpdateAsync(user, ct);
        
        return Result<ResetPasswordOutput>.Success(new ResetPasswordOutput
        {
            Message = "Your password has been reset successfully."
        });
    }
}