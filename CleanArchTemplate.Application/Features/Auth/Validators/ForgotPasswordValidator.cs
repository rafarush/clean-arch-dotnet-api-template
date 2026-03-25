using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Auth.Validators;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordInput>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");
    }
}