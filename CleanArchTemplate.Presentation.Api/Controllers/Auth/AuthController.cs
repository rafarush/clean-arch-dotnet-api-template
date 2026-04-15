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
    IQuerySender querySender,
    IAuthenticationSchemeProvider authenticationSchemeProvider) : BaseApiController(commandSender, querySender)
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
        => Challenge(new AuthenticationProperties { RedirectUri = Url.Action(nameof(OAuthGoogleCallback)) }, "Google");

    [HttpGet(ApiEndpoints.Auth.OAuthGitHub)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> OAuthGitHub()
    {
        var scheme = await authenticationSchemeProvider.GetSchemeAsync("GitHub");
        return Challenge(new AuthenticationProperties { RedirectUri = Url.Action(nameof(OAuthGitHubCallback)) }, scheme.Name);
    }

    [HttpGet("/signin-google")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OAuthGoogleCallback(CancellationToken ct)
        => await HandleOAuthCallback(OAuthProviderType.Google, ct);

    [HttpGet("/signin-github")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OAuthGitHubCallback(CancellationToken ct)
        => await HandleOAuthCallback(OAuthProviderType.GitHub, ct);

    private async Task<IActionResult> HandleOAuthCallback(OAuthProviderType provider, CancellationToken ct)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(provider.ToString());
        if (!authenticateResult.Succeeded)
            return BadRequest("OAuth authentication failed");

        var code = authenticateResult.Properties?.Items["code"];
        if (string.IsNullOrEmpty(code))
            return BadRequest("Authorization code not found");

        return await HandleCommandAsync<OAuthSignInCommand, Result<TokenOutput>>(
            new OAuthSignInCommand(provider, code), ct);
    }

    [HttpPost(ApiEndpoints.Auth.OAuthLinkAccount)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> OAuthLinkAccount([FromBody] OAuthLinkAccountInput input, CancellationToken ct)
        => await HandleCommandAsync<OAuthLinkAccountCommand, Result<bool>>(
            new OAuthLinkAccountCommand(input.UserId, input.Provider, input.Code), ct);
}