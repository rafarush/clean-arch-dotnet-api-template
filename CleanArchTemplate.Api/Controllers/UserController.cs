using CleanArchTemplate.Aplication.Features.User.Models;
using CleanArchTemplate.Aplication.Features.User.Models.Input;
using CleanArchTemplate.Aplication.Features.User.Models.Output;
using CleanArchTemplate.Domain.Users;
using CleanArchTemplate.Infrastructure.Repositories.User;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers;

[ApiController]
public class UserController(IUserRepository userRepository) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;

    [HttpPost(ApiEndpoints.Users.Create)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserInput input, CancellationToken ct)
    {
        var user = input.ToUser();
        var userId = await _userRepository.CreateAsync(user, ct);
        var output = user.ToOutput();
        return CreatedAtAction(nameof(Get), new {id = user.Id}, output);
    }

    [HttpGet(ApiEndpoints.Users.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _userRepository.GetAllAsync(ct);
        var output = users.Select(u => u.ToOutput()).ToList();
        return Ok(output);
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