using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Repositories.Security.Role;
using CleanArchTemplate.SharedKernel.Models.General.Output;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Output;
using Microsoft.AspNetCore.Http;

namespace CleanArchTemplate.Application.Features.Security.Role.Queries;

public sealed record GetRolesQuery() : IQuery<Result<IEnumerable<RoleOutput>>>;


internal class GetRolesQueryHandler(
    IRoleRepository roleRepository
    ) : IQueryHandler<GetRolesQuery, Result<IEnumerable<RoleOutput>>>
{
    public async Task<Result<IEnumerable<RoleOutput>>> Handle(GetRolesQuery query, CancellationToken ct)
    {
        var roles = await roleRepository.GetAllAsync(ct);
        var output = roles.Select(r => r.ToOutput());
        return Result<IEnumerable<RoleOutput>>.Success(output);
    }
}