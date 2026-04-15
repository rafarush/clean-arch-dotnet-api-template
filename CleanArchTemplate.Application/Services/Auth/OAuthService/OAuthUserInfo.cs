namespace CleanArchTemplate.Application.Services.Auth.OAuthService;

public record OAuthUserInfo(string ProviderId, string Email, string Name, string? LastName);
