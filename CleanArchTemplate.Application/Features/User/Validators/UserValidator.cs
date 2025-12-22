using FluentValidation;

namespace CleanArchTemplate.Aplication.Features.User.Validators;

public class UserValidator : AbstractValidator<Domain.Users.User>
{
    public UserValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .MaximumLength(20).WithMessage("Password must not exceed 20 characters");
    }
}