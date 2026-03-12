using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Infrastructure.Repositories.Security.Role;
using CleanArchTemplate.SharedKernel.Models.General.Output;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Output;
using Microsoft.AspNetCore.Http;

namespace CleanArchTemplate.Application.Features.Security.Role.Queries;

public sealed record GetRoleByIdQuery(Guid Id) : IQuery<Result<RoleDetailsOutput>>;


internal class GetRoleByIdQueryHandler(
    IRoleRepository roleRepository
) : IQueryHandler<GetRoleByIdQuery, Result<RoleDetailsOutput>>
{
    public async Task<Result<RoleDetailsOutput>> Handle(GetRoleByIdQuery query, CancellationToken ct)
    {
        var role = await roleRepository.GetAsync(query.Id, ct);
        return role is null ?
            Result<RoleDetailsOutput>.Failure("Role not found", ErrorType.NotFound) :
            Result<RoleDetailsOutput>.Success(role.ToDetailsOutput());
    }
}