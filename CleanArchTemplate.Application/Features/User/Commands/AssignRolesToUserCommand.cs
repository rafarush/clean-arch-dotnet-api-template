using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Repositories.Security.Role;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.User.Commands;

public sealed record AssignRolesToUserCommand(Guid UserId, AssignRolesToUserInput Input) : ICommand<Result<UserDetailsOutput>>;

internal class AssignRolesToUserCommandHandler(
    IRoleRepository roleRepository,
    IUserRepository userRepository,
    IValidator<AssignRolesToUserInput> assignRolesToUserValidator
) : ICommandHandler<AssignRolesToUserCommand, Result<UserDetailsOutput>>
{
    public async Task<Result<UserDetailsOutput>> Handle(AssignRolesToUserCommand command, CancellationToken ct)
    {
        await assignRolesToUserValidator.ValidateAndThrowAsync(command.Input, ct);
        
        var user = await userRepository.GetWithRelationsAsync(command.UserId, ct);
        if (user is null)
            return Result<UserDetailsOutput>.NotFound("User not found");
        
        var distinctRoles = command.Input.RoleIds.Distinct().ToList();
        var roles = await roleRepository.GetByIdsAsync(distinctRoles, ct);
        if (roles.Count < distinctRoles.Count)
            return Result<UserDetailsOutput>.Failure("Some roles id do not exist", ErrorType.Conflict);
        
        var existingRolesIds = user.Roles.Select(r => r.Id).ToHashSet();
        var newRoles = roles.Where(r => !existingRolesIds.Contains(r.Id)).ToList();

        if (newRoles.Count == 0)
            return Result<UserDetailsOutput>.Failure("All roles are already assigned to this user", ErrorType.Conflict);

        var result = await userRepository.AssignRolesToUserAsync(user, roles, ct);

        return Result<UserDetailsOutput>.Success(result.ToDetailsOutput());
    }

}