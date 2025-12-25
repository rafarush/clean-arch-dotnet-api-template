using CleanArchTemplate.Aplication.Abstractions;
using CleanArchTemplate.Aplication.Abstractions.Cqrs;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Query;
using CleanArchTemplate.Aplication.Features.Auth;
using CleanArchTemplate.Aplication.Features.User.Commands;
using CleanArchTemplate.Aplication.Features.User.Queries;
using CleanArchTemplate.Domain.Users;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.General.Output;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers;

public class UserController(
    ICommandSender commandSender,
    IQuerySender querySender) : BaseApiController(commandSender, querySender)
{

    // [Authorize(Policy = PoliciesName.User.Create)]
    [HttpPost(ApiEndpoints.Users.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserInput input, CancellationToken ct)
    => await HandleCreateCommandAsync<CreateUserCommand, CreateUserOutput>(
        new CreateUserCommand(input),
        getActionName: nameof(ApiEndpoints.Users.Get),
        getId: r => r.Value!.Id,
        getOutput: r=> r.Value!.Output,
        ct: ct);
    
    [Authorize(Policy = PoliciesName.User.View)]
    [HttpGet(ApiEndpoints.Users.Get)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    => await HandleQueryAsync<GetUserByIdQuery, Result<UserOutput>>(new GetUserByIdQuery(id), ct);

    
    [Authorize(Policy = PoliciesName.User.View)]
    [HttpGet(ApiEndpoints.Users.Search)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersInput usersInput, CancellationToken ct)
        => await HandleQueryAsync<SearchUsersQuery, Result<PaginatedOutput<UserOutput>>>(
            new SearchUsersQuery(usersInput)
            {
                OffsetField = usersInput.OffsetField,
                OffsetPage = usersInput.OffsetPage,
                Limit = usersInput.Limit,
            }, ct);

    [Authorize(Policy = PoliciesName.User.Update)]
    [HttpPut(ApiEndpoints.Users.Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserInput input,
        CancellationToken ct)
        => await HandleCommandAsync<UpdateUserCommand, Result<UserOutput>>(new UpdateUserCommand(id, input), ct);

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