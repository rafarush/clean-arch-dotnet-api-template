using CleanArchTemplate.Aplication.Features.Security.Policy.Models;
using CleanArchTemplate.Aplication.Features.Security.Policy.Models.Input;
using CleanArchTemplate.Aplication.Features.Security.Policy.Models.Output;
using CleanArchTemplate.Domain.Security;
using CleanArchTemplate.Infrastructure.Repositories.Security.Policy;
using CleanArchTemplate.SharedKernel.Models.Output;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers.Security;

[ApiController]
// [Authorize]
public class PolicyController(IPolicyRepository policyRepository, IValidator<Policy> policyValidator) : ControllerBase
{
    
    [HttpPost(ApiEndpoints.Policies.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePolicy([FromBody] CreatePolicyInput input, CancellationToken ct)
    {
        var policy = input.ToPolicy();
        await policyValidator.ValidateAndThrowAsync(policy, ct);
        var policyId = await policyRepository.CreateAsync(policy, ct);
        var output = policy.ToOutput();
        return CreatedAtAction(nameof(Get), new {id = policyId}, output);
    }
    
    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Policies.GetAll)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IResult> GetAll(CancellationToken ct)
    {
        var policies = await policyRepository.GetAllAsync(ct);
        var output = policies.Select(u => u.ToOutput()).ToList();
        var result = new PaginatedOutput<PolicyOutput>(output, output.Count);
        return Results.Ok(result);
    }
    
    [HttpGet(ApiEndpoints.Policies.Get)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var policy = await policyRepository.GetAsync(id, ct);
        if (policy == null)
            return NotFound();
        var output = policy.ToOutput();
        return Ok(output);
    }
    
    [HttpPut(ApiEndpoints.Policies.Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePolicyInput input, CancellationToken ct)
    {
        var policyToUpdate = await policyRepository.GetAsync(id, ct);
        if (policyToUpdate == null)
            return NotFound();
        
        policyToUpdate.Update(input.Name);
        var policyUpdated = await policyRepository.UpdateAsync(policyToUpdate, ct);
        if (!policyUpdated)
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update policy");
        
        var output = policyToUpdate.ToOutput();
        return Ok(output);
    }

    [HttpDelete(ApiEndpoints.Policies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var deleted = await policyRepository.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound();
        return Ok();
    }
}