namespace CleanArchTemplate.Infrastructure.Services.Auth;

public interface IPasswordHashService
{
    public Task<byte[]> HashPassword(string password);
    public Task<bool> ValidatePassword(byte[] inputPassword, byte[] userPassword);
}