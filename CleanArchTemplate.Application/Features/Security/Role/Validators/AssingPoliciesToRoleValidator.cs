using CleanArchTemplate.SharedKernel.Models.Security.Role.Input;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Security.Role.Validators;

public class AssingPoliciesToRoleValidator : AbstractValidator<AssingPoliciesToRoleInput>
{
    public AssingPoliciesToRoleValidator()
    {
        RuleFor(input => input.PoliciesIds)
            .NotEmpty();
        
    }
}