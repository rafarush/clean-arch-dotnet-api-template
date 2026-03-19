using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Infrastructure.Repositories.Security.Policy;
using CleanArchTemplate.Infrastructure.Repositories.Security.Role;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Input;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Output;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CleanArchTemplate.Application.Features.Security.Role.Commands;

public sealed record CreateRoleCommand(CreateRoleInput Input) : ICommand<Result<CreateRoleOutput>>;

internal class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    IValidator<Domain.Security.Role> roleValidator,
    IPolicyRepository policyRepository
    ) : ICommandHandler<CreateRoleCommand, Result<CreateRoleOutput>>
{
    public async Task<Result<CreateRoleOutput>> Handle(CreateRoleCommand command, CancellationToken ct)
    {
        var exists = await roleRepository.ExistsAsync(command.Input.Name, ct);
        if (exists)
            return Result<CreateRoleOutput>.Failure("Role already exists", ErrorType.Conflict);
        
        var role = command.Input.ToRole();
        await roleValidator.ValidateAndThrowAsync(role, ct);

        if (command.Input.Policies is not null)
        {
            var distinctPolicies = command.Input.Policies.Distinct().ToList();
            var policies = await policyRepository.GetByIdsAsync(distinctPolicies, ct);
            if (policies.Count < distinctPolicies.Count)
                return Result<CreateRoleOutput>.Failure("Some policies id do not exist", ErrorType.Conflict);
            
            role.Policies.AddRange(policies);
        }
          
        
        var id = await roleRepository.CreateAsync(role, ct);
        return id != Guid.Empty
            ? Result<CreateRoleOutput>.Success(new(id, role.ToOutput()), StatusCodes.Status201Created)
            : Result<CreateRoleOutput>.Failure("Role could not be created", ErrorType.Conflict);
    }
}