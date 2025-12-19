using FluentValidation;

namespace CleanArchTemplate.Aplication.Features.Security.Policy;

using Domain.Security;

public class PolicyValidator :  AbstractValidator<Policy>
{
    public PolicyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(1, 30)
            .WithMessage("Name must be between 1 and 30 characters");
    }
}