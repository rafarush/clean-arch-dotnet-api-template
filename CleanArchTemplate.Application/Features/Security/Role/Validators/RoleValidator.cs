using FluentValidation;

namespace CleanArchTemplate.Application.Features.Security.Role.Validators;

public class RoleValidator : AbstractValidator<Domain.Security.Role>
{
    public RoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long")
            .MaximumLength(20).WithMessage("Name must not exceed 20 characters");
        RuleFor(x => x.Description)
            .MinimumLength(5).WithMessage("Description must be at least 5 characters long")
            .MaximumLength(200).WithMessage("Description must not exceed 200 characters");
    }
}