using CleanArchTemplate.SharedKernel.Models.User.Input;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.User.Validators;

public class AssignRolesToUserValidator : AbstractValidator<AssignRolesToUserInput>
{
    public AssignRolesToUserValidator()
    {
        RuleFor(input => input.RoleIds)
            .NotEmpty();
        
    }
}