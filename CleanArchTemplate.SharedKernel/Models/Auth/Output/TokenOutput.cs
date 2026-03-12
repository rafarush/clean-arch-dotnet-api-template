namespace CleanArchTemplate.SharedKernel.Models.Auth.Output;

public sealed record TokenOutput(
    string AccessToken,
    string RefreshToken,
    Guid Id,  
    string Email,
    IEnumerable<string> RoleNames);