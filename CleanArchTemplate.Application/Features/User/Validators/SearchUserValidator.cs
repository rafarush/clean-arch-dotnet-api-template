using CleanArchTemplate.Aplication.Features.User.Queries;
using FluentValidation;

namespace CleanArchTemplate.Aplication.Features.User.Validators;

public class SearchUserValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUserValidator()
    {
        RuleFor(x => x.OffsetPage)
            .GreaterThanOrEqualTo(1).WithMessage("Offset field must be greater than or equal to 1");
        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(1).WithMessage("Limit field must be greater than or equal to 1");
        RuleFor(x => x.IsOffsetFieldValid())
            .Equal(true)
            .OverridePropertyName("OffsetField")
            .WithMessage("Offset field is invalid");
    }
}