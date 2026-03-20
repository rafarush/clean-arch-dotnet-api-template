using System.Security.Cryptography;

namespace CleanArchTemplate.Infrastructure.Services.Auth.PasswordHashService;

public sealed class PasswordHashService : IPasswordHashService
{
    private const int SaltSize = 16;    
    private const int HashSize = 32;       
    private const int Iterations = 350_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public Task<byte[]> HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        
        var result = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);

        return Task.FromResult(result);
    }

    public Task<bool> ValidatePassword(string inputPassword, byte[] storedPassword)
    {
        var salt = new byte[SaltSize];
        var storedHash = new byte[HashSize];
        Buffer.BlockCopy(storedPassword, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(storedPassword, SaltSize, storedHash, 0, HashSize);
        
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, Iterations, Algorithm, HashSize);

        return Task.FromResult(CryptographicOperations.FixedTimeEquals(inputHash, storedHash));
    }
}