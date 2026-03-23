namespace CleanArchTemplate.Infrastructure.Services.Auth.PasswordHashService;

public interface IPasswordHashService
{
    public Task<string> HashPassword(string password);
    public Task<bool> ValidatePassword(string inputPassword, string userPassword);
}