namespace CleanArchTemplate.Infrastructure.Services.Auth.PasswordHashService;

public interface IPasswordHashService
{
    public Task<byte[]> HashPassword(string password);
    public Task<bool> ValidatePassword(string inputPassword, byte[] userPassword);
}