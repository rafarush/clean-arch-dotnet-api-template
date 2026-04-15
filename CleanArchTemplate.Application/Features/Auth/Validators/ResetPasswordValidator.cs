using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Auth.Validators;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordInput>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}