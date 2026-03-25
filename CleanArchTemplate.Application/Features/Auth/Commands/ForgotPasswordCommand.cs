using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.VerificationLinkService;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService.Models;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Auth.Commands;

public sealed record ForgotPasswordCommand(ForgotPasswordInput Input) : ICommand<Result<ForgotPasswordOutput>>;

internal sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository, 
    IVerificationTokenService verificationTokenService,
    IValidator<ForgotPasswordInput> forgotPasswordCommandValidator
    ) : ICommandHandler<ForgotPasswordCommand, Result<ForgotPasswordOutput>>
{
    public async Task<Result<ForgotPasswordOutput>> Handle(ForgotPasswordCommand command, CancellationToken ct)
    {
        await forgotPasswordCommandValidator.ValidateAndThrowAsync(command.Input, ct);
        
        var user = await userRepository.GetByEmailAsync(command.Input.Email, ct);
        if (user is not null)
        {
            var tokenInfo = verificationTokenService.GenerateLink(user, TokenMotive.ResetPassword);
            await userRepository.UpdateAsync(user, ct);
            
        }
        // TODO: Send Reset Password email
        return Result<ForgotPasswordOutput>.Success(new ForgotPasswordOutput()
        {
            Message = "If email exists, you will receive an email to reset your password."
        });
        // Todo: Implement Reset Password endpoint
    }
}