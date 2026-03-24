using CleanArchTemplate.Application.Abstractions;
using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Features.Auth.Commands;
using CleanArchTemplate.Application.Features.Auth.Services;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers.Auth;


public class AuthController(
    ICommandSender commandSender,
    IQuerySender querySender) : BaseApiController(commandSender, querySender)
{
    [HttpPost(ApiEndpoints.Auth.SignIn)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignIn([FromBody] SignInInput input, CancellationToken ct)
        => await HandleCommandAsync<SignInCommand, Result<TokenOutput>>(new SignInCommand(input), ct);
    
    [HttpPost(ApiEndpoints.Auth.Refresh)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignIn(string token, CancellationToken ct)
        => await HandleCommandAsync<RefreshTokenCommand, Result<TokenOutput>>(new RefreshTokenCommand(token), ct);
    
    [HttpPost(ApiEndpoints.Auth.SignUp)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> SignUp([FromBody] SignUpInput input, CancellationToken ct)
        => await HandleCreateCommandAsync<SignUpCommand, CreateUserOutput>(
            new SignUpCommand(input),
            routeName: ApiEndpoints.Users.GetRouteName,
            getId: r => r.Value!.Id,
            getOutput: r=> r.Value!.Output,
            ct: ct);
    
    
    //TODO: Add Email Verification feat (EmailService first)
}