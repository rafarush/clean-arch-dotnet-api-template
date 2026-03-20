using CleanArchTemplate.Application.Abstractions;
using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Features.Auth;
using CleanArchTemplate.Application.Features.Security.Policy;
using CleanArchTemplate.Application.Features.Security.Policy.Commands;
using CleanArchTemplate.Application.Features.Security.Policy.Queries;
using CleanArchTemplate.Domain.Security;
using CleanArchTemplate.Infrastructure.Repositories.Security.Policy;
using CleanArchTemplate.SharedKernel.Models.General.Output;
using CleanArchTemplate.SharedKernel.Models.Security.Policy.Input;
using CleanArchTemplate.SharedKernel.Models.Security.Policy.Output;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers.Security;

public class PolicyController(
    ICommandSender commandSender,
    IQuerySender querySender) : BaseApiController(commandSender, querySender)
{
    
    [HttpPost(ApiEndpoints.Policies.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreatePolicyInput input, CancellationToken ct)
        => await HandleCreateCommandAsync<CreatePolicyCommand, CreatePolicyOutput>(
            new CreatePolicyCommand(input),
            getActionName: nameof(ApiEndpoints.Policies.Get),
            getId: r => r.Value!.Id,
            getOutput: r=> r.Value!.Output,
            ct: ct);
    
    [Authorize(Policy = PoliciesName.Policy.View)]
    [HttpGet(ApiEndpoints.Policies.GetAll)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => await HandleQueryAsync<GetPoliciesQuery, Result<IEnumerable<PolicyOutput>>>(new GetPoliciesQuery(), ct);
    
    [Authorize(Policy = PoliciesName.Policy.View)]
    [HttpGet(ApiEndpoints.Policies.Get)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
        => await HandleQueryAsync<GetPolicyByIdQuery, Result<PolicyOutput>>(new GetPolicyByIdQuery(id), ct);
    
    [Authorize(Policy = PoliciesName.Policy.Delete)]
    [HttpDelete(ApiEndpoints.Policies.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken ct)
        => await HandleCommandAsync<DeletePolicyCommand, Result<PolicyOutput>>(new DeletePolicyCommand(id), ct);
}