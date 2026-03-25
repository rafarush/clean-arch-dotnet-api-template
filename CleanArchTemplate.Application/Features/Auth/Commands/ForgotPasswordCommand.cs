using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.VerificationLinkService;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService.Models;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Auth.Commands;

public sealed record ForgotPasswordCommand(ForgotPasswordInput Input) : ICommand<Result<string>>;

internal sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository, 
    IVerificationTokenService verificationTokenService,
    IValidator<ForgotPasswordInput> forgotPasswordCommandValidator
    ) : ICommandHandler<ForgotPasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ForgotPasswordCommand command, CancellationToken ct)
    {
        await forgotPasswordCommandValidator.ValidateAndThrowAsync(command.Input, ct);
        
        var user = await userRepository.GetByEmailAsync(command.Input.Email, ct);
        if (user is not null)
        {
            var tokenInfo = verificationTokenService.GenerateLink(user, TokenMotive.ResetPassword);
            await userRepository.UpdateAsync(user, ct);
            
        }
        // TODO: Use a better response
        return Result<string>.Success("If email exists, you will receive an email to reset your password.");
    }
}