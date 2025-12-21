using CleanArchTemplate.Domain.Security;

namespace CleanArchTemplate.Aplication.Features.Auth.Models.Output;

public sealed record TokenOutput(
    string AccessToken,
    string RefreshToken,
    Guid Id,  
    string Email, 
    IEnumerable<string> PolicyNames,
    IEnumerable<string> RoleNames);