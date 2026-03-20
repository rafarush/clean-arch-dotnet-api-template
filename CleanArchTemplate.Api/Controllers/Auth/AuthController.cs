using CleanArchTemplate.Application.Abstractions;
using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Features.Auth.Commands;
using CleanArchTemplate.Application.Features.Auth.Services;
using CleanArchTemplate.Domain.Users;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;
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
    
    //TODO Add SignUp feat
    //TODO Add Email Verification feat (EmailService first)
    
}