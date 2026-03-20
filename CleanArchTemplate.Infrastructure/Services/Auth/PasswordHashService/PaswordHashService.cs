using System.Security.Cryptography;
using CleanArchTemplate.Infrastructure.Services.Auth.Options;
using Microsoft.Extensions.Options;

namespace CleanArchTemplate.Infrastructure.Services.Auth.PasswordHashService;

public sealed class PasswordHashService(
    IOptions<PasswordHashServiceOptions> options) : IPasswordHashService
{
    public Task<byte[]> HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(options.Value.SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, options.Value.Iterations, 
            options.Value.Algorithm, options.Value.HashSize);
        
        var result = new byte[options.Value.SaltSize + options.Value.HashSize];
        Buffer.BlockCopy(salt, 0, result, 0, options.Value.SaltSize);
        Buffer.BlockCopy(hash, 0, result, options.Value.SaltSize, options.Value.HashSize);

        return Task.FromResult(result);
    }

    public Task<bool> ValidatePassword(string inputPassword, byte[] storedPassword)
    {
        var salt = new byte[options.Value.SaltSize];
        var storedHash = new byte[options.Value.HashSize];
        Buffer.BlockCopy(storedPassword, 0, salt, 0, options.Value.SaltSize);
        Buffer.BlockCopy(storedPassword, options.Value.SaltSize, storedHash, 0, options.Value.HashSize);
        
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, options.Value.Iterations, 
            options.Value.Algorithm, options.Value.HashSize);

        return Task.FromResult(CryptographicOperations.FixedTimeEquals(inputHash, storedHash));
    }
}