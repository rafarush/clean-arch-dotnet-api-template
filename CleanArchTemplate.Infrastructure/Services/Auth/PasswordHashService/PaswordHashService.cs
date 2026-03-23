using System.Security.Cryptography;
using CleanArchTemplate.Infrastructure.Services.Auth.Options;
using Microsoft.Extensions.Options;

namespace CleanArchTemplate.Infrastructure.Services.Auth.PasswordHashService;

public sealed class PasswordHashService : IPasswordHashService
{
    public Task<string> HashPassword(string password)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        return Task.FromResult(hash);
    }

    public Task<bool> ValidatePassword(string inputPassword, string storedHash)
    {
        var isValid = BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
        return Task.FromResult(isValid);
    }
}