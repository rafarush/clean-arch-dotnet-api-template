using FluentValidation;

namespace CleanArchTemplate.Application.Features.User.Validators;

public class UserValidator : AbstractValidator<Domain.User.User>
{
    public UserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");
        // RuleFor(x => x.Password)
        //     .NotEmpty().WithMessage("Password is required")
        //     .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
        //     .MaximumLength(20).WithMessage("Password must not exceed 20 characters");
    }
}