using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Repositories.Security.Role;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Output;
using Microsoft.AspNetCore.Http;

namespace CleanArchTemplate.Application.Features.Security.Role.Commands;

public sealed record DeleteRoleCommand(Guid Id) : ICommand<Result<RoleOutput>>;


internal sealed class DeleteRoleCommandHandler(
    IRoleRepository roleRepository
) : ICommandHandler<DeleteRoleCommand, Result<RoleOutput>>
{
    public async Task<Result<RoleOutput>> Handle(DeleteRoleCommand command, CancellationToken ct)
    {
        var role = await roleRepository.GetAsync(command.Id, ct);
        if (role == null)
            return Result<RoleOutput>.Failure("Role not found", ErrorType.NotFound);
        var deleted = await roleRepository.DeleteAsync(command.Id, ct);
        return !deleted 
            ? Result<RoleOutput>.Failure("Role could not be deleted", ErrorType.Conflict) 
            : Result<RoleOutput>.Success(role.ToOutput(), "Role deleted successfully", StatusCodes.Status204NoContent);
    }
}