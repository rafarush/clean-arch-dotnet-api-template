using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Auth.Validators;

public class SignInValidator : AbstractValidator<SignInInput>
{
    public SignInValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
        // .MinimumLength(8).WithMessage("Password must be at least 8 characters")
        // .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
        // .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}