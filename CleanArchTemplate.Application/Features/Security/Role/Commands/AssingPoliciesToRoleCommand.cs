using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Infrastructure.Repositories.Security.Policy;
using CleanArchTemplate.Infrastructure.Repositories.Security.Role;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Input;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Security.Role.Commands;

public sealed record AssingPoliciesToRoleCommand(Guid RoleId, AssingPoliciesToRoleInput Input) : ICommand<Result<RoleOutput>>;

internal class AssingPoliciesToRoleCommandHandler(
    IRoleRepository roleRepository,
    IPolicyRepository policyRepository,
    IValidator<AssingPoliciesToRoleInput> assingPoliciesToRoleInputValidator
    ) : ICommandHandler<AssingPoliciesToRoleCommand, Result<RoleOutput>>
{
    public async Task<Result<RoleOutput>> Handle(AssingPoliciesToRoleCommand command, CancellationToken ct)
    {
        await assingPoliciesToRoleInputValidator.ValidateAndThrowAsync(command.Input, ct);
        
        var role = await roleRepository.GetAsync(command.RoleId, ct);
        if (role is null)
            return Result<RoleOutput>.NotFound("Role not found");
        
        var distinctPolicies = command.Input.PoliciesIds.Distinct().ToList();
        var policies = await policyRepository.GetByIdsAsync(distinctPolicies, ct);
        if (policies.Count < distinctPolicies.Count)
            return Result<RoleOutput>.Failure("Some policies id do not exist", ErrorType.Conflict);
        
        var existingPolicyIds = role.Policies.Select(p => p.Id).ToHashSet();
        var newPolicies = policies.Where(p => !existingPolicyIds.Contains(p.Id)).ToList();

        if (newPolicies.Count == 0)
            return Result<RoleOutput>.Failure("All policies are already assigned to this role", ErrorType.Conflict);

        var result = await roleRepository.AssignPoliciesToRoleAsync(role, policies, ct);

        return Result<RoleOutput>.Success(result.ToOutput());
    }

}