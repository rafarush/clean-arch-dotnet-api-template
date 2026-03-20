using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Infrastructure.Repositories.Security.Role;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.User.Commands;

public sealed record AssignRolesToUserCommand(Guid UserId, AssignRolesToUserInput Input) : ICommand<Result<UserOutput>>;

internal class AssignRolesToUserCommandHandler(
    IRoleRepository roleRepository,
    IUserRepository userRepository,
    IValidator<AssignRolesToUserInput> assignRolesToUserValidator
) : ICommandHandler<AssignRolesToUserCommand, Result<UserOutput>>
{
    public async Task<Result<UserOutput>> Handle(AssignRolesToUserCommand command, CancellationToken ct)
    {
        await assignRolesToUserValidator.ValidateAndThrowAsync(command.Input, ct);
        
        var user = await userRepository.GetAsync(command.UserId, ct);
        if (user is null)
            return Result<UserOutput>.NotFound("User not found");
        
        var distinctRoles = command.Input.RoleIds.Distinct().ToList();
        var roles = await roleRepository.GetByIdsAsync(distinctRoles, ct);
        if (roles.Count < distinctRoles.Count)
            return Result<UserOutput>.Failure("Some roles id do not exist", ErrorType.Conflict);
        
        var existingRolesIds = user.Roles.Select(r => r.Id).ToHashSet();
        var newRoles = roles.Where(r => !existingRolesIds.Contains(r.Id)).ToList();

        if (newRoles.Count == 0)
            return Result<UserOutput>.Failure("All roles are already assigned to this user", ErrorType.Conflict);

        var result = await userRepository.AssignRolesToUserAsync(user, roles, ct);

        return Result<UserOutput>.Success(result.ToOutput());
    }

}