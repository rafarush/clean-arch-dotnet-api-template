using CleanArchTemplate.Application.Features.User.Models;
using CleanArchTemplate.Application.Features.User.Models.Input;
using CleanArchTemplate.Application.Features.User.Models.Output;
using CleanArchTemplate.Domain.Users;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.Output;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers;

[ApiController]
public class UserController(IUserRepository userRepository, IValidator<User> userValidator) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;

    [HttpPost(ApiEndpoints.Users.Create)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserInput input, CancellationToken ct)
    {
        var user = input.ToUser();
        await userValidator.ValidateAndThrowAsync(user, ct);
        var userId = await _userRepository.CreateAsync(user, ct);
        var output = user.ToOutput();
        return CreatedAtAction(nameof(Get), new {id = userId}, output);
    }

    [HttpGet(ApiEndpoints.Users.GetAll)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IResult> GetAll(CancellationToken ct)
    {
        var users = await _userRepository.GetAllAsync(ct);
        var output = users.Select(u => u.ToOutput()).ToList();
        var result = new PaginatedOutput<UserOutput>(output, output.Count);
        return Results.Ok(result);
    }

    [HttpGet(ApiEndpoints.Users.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        var user = await _userRepository.GetAsync(id, ct);
        if (user == null)
            return NotFound();
        var output = user.ToOutput();
        return Ok(output);
    }

    [HttpPut(ApiEndpoints.Users.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserInput input, CancellationToken ct)
    {
        var user = input.ToUser(id);
        await userValidator.ValidateAndThrowAsync(user, ct);
        var userUpdated = await _userRepository.UpdateAsync(user, ct);
        if (!userUpdated)
            return NotFound();
        var output = user.ToOutput();
        return Ok(output);
    }

    [HttpDelete(ApiEndpoints.Users.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var deleted = await _userRepository.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound();
        return Ok();
    }
}