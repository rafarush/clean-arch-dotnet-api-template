using CleanArchTemplate.Aplication.Features.Auth.Models.Input;
using CleanArchTemplate.Aplication.Features.Auth.Services;
using CleanArchTemplate.Domain.Users;
using CleanArchTemplate.Infrastructure.Repositories.User;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Api.Controllers.Auth;

[ApiController]
public class AuthController(/*IUserRepository userRepository*/IAuthService authService): ControllerBase
{
    [HttpPost(ApiEndpoints.Auth.SignIn)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignIn([FromBody] SignInInput input, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await authService.SignInAsync(input.Email, input.Password, ct);
        
        if (!result.IsSuccess)
            return Unauthorized(new { result.ErrorMessage });

        return Ok(result.Token);
    }
    // public async Task<IActionResult> SignIn([FromBody] SignInInput input, CancellationToken ct)
    // {
    //     var user = await VerifyCredentialsAsync(input, ct);
    //     
    //     if (user == null)
    //         return Unauthorized(new { Message = "Incorrect email or password" });
    //
    //     var tokenInput = new TokenInput
    //     {
    //         Email = user.Email,
    //         Policies = user.Roles.SelectMany(static x => x.Policies)
    //             .ToList(),
    //         User = user.Id,
    //         Roles = user.Roles
    //     };
    //     var token = jwtService.CreateToken(tokenInput);
    //     return Ok(token);
    // }
    
    
    // private async Task<User?> VerifyCredentialsAsync(SignInInput input, CancellationToken ct) 
    // {
    //     var user = await userRepository.GetByEmailAsync(input.Email, ct);
    //     if (user is null)
    //         return null;
    //     return input.Password.Equals(user.Password, StringComparison.Ordinal)
    //         ? await Task.FromResult(user)
    //         : null;
    // }
}