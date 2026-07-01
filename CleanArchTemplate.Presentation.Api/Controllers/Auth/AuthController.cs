using System.Security.Claims;
using CleanArchTemplate.Application.Abstractions;
using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Features.Auth.Commands;
using CleanArchTemplate.Application.Features.Auth.Queries;
using CleanArchTemplate.Domain.AuthProvider;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers.Auth;

public class AuthController(ICommandSender commandSender, IQuerySender querySender)
    : BaseApiController(commandSender, querySender)
{
    private const string OAuthStateCookie = "oauth_state";

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
    public async Task<IActionResult> Refresh(string token, CancellationToken ct)
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
            getOutput: r => r.Value!.Output,
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> OAuthGoogle(CancellationToken ct)
        => await BuildOAuthRedirect(OAuthProviderType.Google, ct);

    [HttpGet(ApiEndpoints.Auth.OAuthGitHub)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> OAuthGitHub(CancellationToken ct)
        => await BuildOAuthRedirect(OAuthProviderType.GitHub, ct);

    [HttpGet(ApiEndpoints.Auth.OAuthGoogleCallback)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OAuthGoogleCallback(
        [FromQuery] string? code,
        [FromQuery] string? state,
        [FromQuery] string? error,
        CancellationToken ct)
        => await HandleOAuthCallback(OAuthProviderType.Google, code, state, error, ct);

    [HttpGet(ApiEndpoints.Auth.OAuthGitHubCallback)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OAuthGitHubCallback(
        [FromQuery] string? code,
        [FromQuery] string? state,
        [FromQuery] string? error,
        CancellationToken ct)
        => await HandleOAuthCallback(OAuthProviderType.GitHub, code, state, error, ct);

    [Authorize]
    [HttpPost(ApiEndpoints.Auth.OAuthLinkAccount)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> OAuthLinkAccount([FromBody] OAuthLinkAccountInput input, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await HandleCommandAsync<OAuthLinkAccountCommand, Result<bool>>(
            new OAuthLinkAccountCommand(userId, input.Provider, input.Code), ct);
    }

    private async Task<IActionResult> BuildOAuthRedirect(OAuthProviderType provider, CancellationToken ct)
    {
        var result = await QuerySender.SendAsync(new GetOAuthUrlQuery(provider), ct);
        if (!result.IsSuccess)
            return StatusCode(StatusCodes.Status500InternalServerError);

        Response.Cookies.Append(OAuthStateCookie, result.Value!.State, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = true,
            MaxAge = TimeSpan.FromMinutes(10)
        });

        return Redirect(result.Value.Url);
    }

    private async Task<IActionResult> HandleOAuthCallback(
        OAuthProviderType provider,
        string? code,
        string? state,
        string? error,
        CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(error))
            return BadRequest("OAuth authorization was denied");

        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
            return BadRequest("Missing required OAuth parameters");

        var cookieState = Request.Cookies[OAuthStateCookie];
        if (cookieState != state)
            return BadRequest("Invalid OAuth state");

        Response.Cookies.Delete(OAuthStateCookie);

        return await HandleCommandAsync<OAuthSignInCommand, Result<TokenOutput>>(
            new OAuthSignInCommand(provider, code), ct);
    }
}
