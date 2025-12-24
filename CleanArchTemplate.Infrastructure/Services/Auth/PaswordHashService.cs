using System.Security.Cryptography;
using System.Text;

namespace CleanArchTemplate.Infrastructure.Services.Auth;

public sealed class PasswordHashService : IPasswordHashService
{
    public Task<byte[]> HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        return Task.FromResult(SHA256.HashData(bytes));
    }

    public Task<bool> ValidatePassword(byte[] inputPassword, byte[] userPassword) 
        => Task.FromResult(inputPassword.SequenceEqual(userPassword));
}