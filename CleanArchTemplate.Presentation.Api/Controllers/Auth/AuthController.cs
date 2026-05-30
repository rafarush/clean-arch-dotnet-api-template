using CleanArchTemplate.Application.Abstractions;
using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Features.Auth.Commands;
using CleanArchTemplate.Application.Services.Auth.OAuthService;
using CleanArchTemplate.Domain.AuthProvider;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using Microsoft.AspNetCore.Authentication;
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
    
    
    [HttpPost(ApiEndpoints.Auth.VerifyEmail)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail(string link, CancellationToken ct)
        => await HandleCommandAsync<VerifyEmailCommand, Result<TokenOutput>>(new VerifyEmailCommand(link), ct);
    
    [HttpPost(ApiEndpoints.Auth.ForgotPassword)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordInput input, CancellationToken ct)
        => await HandleCommandAsync<ForgotPasswordCommand, Result<ForgotPasswordOutput>>(new ForgotPasswordCommand(input), ct);
    
    [HttpPost(ApiEndpoints.Auth.ResetPassword)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordInput input, CancellationToken ct)
        => await HandleCommandAsync<ResetPasswordCommand, Result<ResetPasswordOutput>>(new ResetPasswordCommand(input), ct);

    [HttpGet(ApiEndpoints.Auth.OAuthGoogle)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public IActionResult OAuthGoogle()
        => Challenge(new AuthenticationProperties { RedirectUri = "/api/auth/oauth/callback" }, "Google");

    [HttpGet(ApiEndpoints.Auth.OAuthGitHub)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public IActionResult OAuthGitHub()
        => Challenge(new AuthenticationProperties { RedirectUri = "/api/auth/oauth/callback" }, "GitHub");

    [HttpGet(ApiEndpoints.Auth.OAuthCallback)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OAuthCallback(
        [FromQuery] string? provider,
        [FromQuery] string? email,
        [FromQuery] string? name,
        [FromQuery] string? lastName,
        [FromQuery] string? providerId,
        [FromQuery] string? error,
        CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(error))
            return BadRequest($"OAuth error: {error}");
        
        if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(providerId))
            return BadRequest("Missing required parameters");

        var providerType = Enum.Parse<OAuthProviderType>(provider, ignoreCase: true);
        return await HandleCommandAsync<OAuthSignInCommand, Result<TokenOutput>>(
            new OAuthSignInCommand(providerType, email, name, lastName, providerId), ct);
    }

    [HttpPost(ApiEndpoints.Auth.OAuthEmailRequired)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OAuthEmailRequired([FromBody] OAuthEmailRequiredInput input, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(input.Provider) || string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Name) || string.IsNullOrEmpty(input.ProviderId))
            return BadRequest("Missing required parameters");

        var providerType = Enum.Parse<OAuthProviderType>(input.Provider, ignoreCase: true);
        return await HandleCommandAsync<OAuthSignInCommand, Result<TokenOutput>>(
            new OAuthSignInCommand(providerType, input.Email, input.Name, input.LastName, input.ProviderId), ct);
    }

    [HttpPost(ApiEndpoints.Auth.OAuthLinkAccount)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> OAuthLinkAccount([FromBody] OAuthLinkAccountInput input, CancellationToken ct)
        => await HandleCommandAsync<OAuthLinkAccountCommand, Result<bool>>(
            new OAuthLinkAccountCommand(input.UserId, input.Provider, input.Code), ct);
}