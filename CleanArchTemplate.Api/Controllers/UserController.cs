using CleanArchTemplate.Aplication.Abstractions;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;
using CleanArchTemplate.Aplication.Features.User.Commands;
using CleanArchTemplate.Aplication.Features.User.Models.Input;
using CleanArchTemplate.Aplication.Features.User.Models.Output;
using CleanArchTemplate.Aplication.Features.User.Queries;
using CleanArchTemplate.Application.Features.User.Models;
using CleanArchTemplate.Application.Features.User.Models.Output;
using CleanArchTemplate.Domain.Users;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.Output;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers;

// [Authorize]
public class UserController(
    ICommandSender commandSender,
    IQuerySender querySender) : BaseApiController(commandSender, querySender)
{

    [HttpPost(ApiEndpoints.Users.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserInput input, CancellationToken ct)
    => await HandleCreateCommandAsync<CreateUserCommand, CreateUserOutput>(
        new CreateUserCommand(input),
        getActionName: nameof(ApiEndpoints.Users.Get),
        getId: r => r.Id,
        getOutput: r=> r.Output,
        ct: ct);
    
    [HttpGet(ApiEndpoints.Users.Get)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    => await HandleQueryAsync<GetUserByIdQuery, UserOutput?>(new  GetUserByIdQuery(id), ct);

    // [AllowAnonymous]
    // [HttpGet(ApiEndpoints.Users.GetAll)]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IResult> GetAll(CancellationToken ct)
    // {
    //     var users = await userRepository.GetAllAsync(ct);
    //     var output = users.Select(u => u.ToOutput()).ToList();
    //     var result = new PaginatedOutput<UserOutput>(output, output.Count);
    //     return Results.Ok(result);
    // }
    //
    // [HttpGet(ApiEndpoints.Users.Get)]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    // {
    //     var user = await userRepository.GetAsync(id, ct);
    //     if (user == null)
    //         return NotFound();
    //     var output = user.ToOutput();
    //     return Ok(output);
    // }
    //
    // [HttpPut(ApiEndpoints.Users.Update)]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserInput input, CancellationToken ct)
    // {
    //     var user = input.ToUser(id);
    //     await userValidator.ValidateAndThrowAsync(user, ct);
    //     var userUpdated = await userRepository.UpdateAsync(user, ct);
    //     if (!userUpdated)
    //         return NotFound();
    //     var output = user.ToOutput();
    //     return Ok(output);
    // }
    //
    // [HttpDelete(ApiEndpoints.Users.Delete)]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    // {
    //     var deleted = await userRepository.DeleteAsync(id, ct);
    //     if (!deleted)
    //         return NotFound();
    //     return Ok();
    // }
}