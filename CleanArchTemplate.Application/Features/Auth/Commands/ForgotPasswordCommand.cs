using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.VerificationLinkService;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService;
using CleanArchTemplate.Application.Services.Auth.VerificationTokenService.Models;
using CleanArchTemplate.Application.Services.Email;
using CleanArchTemplate.Application.Services.Email.Abstractions;
using CleanArchTemplate.Application.Services.Email.TemplateModels;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Auth.Commands;

public sealed record ForgotPasswordCommand(ForgotPasswordInput Input) : ICommand<Result<ForgotPasswordOutput>>;

internal sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository, 
    IVerificationTokenService verificationTokenService,
    IValidator<ForgotPasswordInput> forgotPasswordCommandValidator,
    IEmailService emailService
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
            await emailService.SendWithTemplateAsync(
                new EmailMessage(user.Email, "Confirm your email"),
                templateKey: "Auth/ResetPassswordEmailTemplate",
                model: new ResetPasswordEmailModel(user.Name + " " + user.LastName, tokenInfo,
                    verificationTokenService.GetTokenLifeInMinutes()),
                ct
            );
        }
        
        return Result<ForgotPasswordOutput>.Success(new ForgotPasswordOutput()
        {
            Message = "If email exists, you will receive an email to reset your password."
        });
        // Todo: Implement Reset Password endpoint
    }
}