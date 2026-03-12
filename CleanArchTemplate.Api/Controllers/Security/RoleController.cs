using CleanArchTemplate.Application.Abstractions;
using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Features.Auth;
using CleanArchTemplate.Application.Features.Security.Role.Commands;
using CleanArchTemplate.Application.Features.Security.Role.Queries;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Input;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers.Security;

public class RoleController(
    ICommandSender commandSender,
    IQuerySender querySender) : BaseApiController(commandSender, querySender)
{
    [Authorize(Policy = PoliciesName.Role.Create)]
    [HttpPost(ApiEndpoints.Roles.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateRoleInput input, CancellationToken ct)
        => await HandleCreateCommandAsync<CreateRoleCommand, CreateRoleOutput>(
            new CreateRoleCommand(input),
            getActionName: nameof(ApiEndpoints.Roles.Get),
            getId: r => r.Value!.Id,
            getOutput: r=> r.Value!.Output,
            ct: ct);
    
    [Authorize(Policy = PoliciesName.Role.View)]
    [HttpGet(ApiEndpoints.Roles.GetAll)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles(CancellationToken ct)
    => await HandleQueryAsync<GetRolesQuery, Result<IEnumerable<RoleOutput>>>(new GetRolesQuery(), ct);
    
    [Authorize(Policy = PoliciesName.Role.View)]
    [HttpGet(ApiEndpoints.Roles.Get)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoleById([FromRoute] Guid id, CancellationToken ct)
        => await HandleQueryAsync<GetRoleByIdQuery, Result<RoleDetailsOutput>>(new GetRoleByIdQuery(id), ct);
}