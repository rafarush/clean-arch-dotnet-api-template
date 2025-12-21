using CleanArchTemplate.Aplication.Features.User.Models.Input;
using CleanArchTemplate.Application.Features.User.Models;
using CleanArchTemplate.Application.Features.User.Models.Output;
using CleanArchTemplate.Domain.Users;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.Output;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers;

[ApiController]
// [Authorize]
public class UserController(IUserRepository userRepository, IValidator<User> userValidator) : ControllerBase
{

    [HttpPost(ApiEndpoints.Users.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserInput input, CancellationToken ct)
    {
        var user = input.ToUser();
        await userValidator.ValidateAndThrowAsync(user, ct);
        var userId = await userRepository.CreateAsync(user, ct);
        var output = user.ToOutput();
        return CreatedAtAction(nameof(Get), new {id = userId}, output);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Users.GetAll)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IResult> GetAll(CancellationToken ct)
    {
        var users = await userRepository.GetAllAsync(ct);
        var output = users.Select(u => u.ToOutput()).ToList();
        var result = new PaginatedOutput<UserOutput>(output, output.Count);
        return Results.Ok(result);
    }

    [HttpGet(ApiEndpoints.Users.Get)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var user = await userRepository.GetAsync(id, ct);
        if (user == null)
            return NotFound();
        var output = user.ToOutput();
        return Ok(output);
    }

    [HttpPut(ApiEndpoints.Users.Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserInput input, CancellationToken ct)
    {
        var user = input.ToUser(id);
        await userValidator.ValidateAndThrowAsync(user, ct);
        var userUpdated = await userRepository.UpdateAsync(user, ct);
        if (!userUpdated)
            return NotFound();
        var output = user.ToOutput();
        return Ok(output);
    }

    [HttpDelete(ApiEndpoints.Users.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var deleted = await userRepository.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound();
        return Ok();
    }
}