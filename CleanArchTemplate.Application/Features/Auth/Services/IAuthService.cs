using CleanArchTemplate.Aplication.Features.Auth.Models.Output;

namespace CleanArchTemplate.Aplication.Features.Auth.Services;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(string email, string password, CancellationToken ct);
}

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public TokenOutput? Token { get; set; }
    public Domain.Users.User? User { get; set; }
    public string? ErrorMessage { get; set; }
    
    public static AuthResult Success(TokenOutput token, Domain.Users.User user)
        => new() { IsSuccess = true, Token = token, User = user };
    
    public static AuthResult Failure(string errorMessage)
        => new() { IsSuccess = false, ErrorMessage = errorMessage };
}