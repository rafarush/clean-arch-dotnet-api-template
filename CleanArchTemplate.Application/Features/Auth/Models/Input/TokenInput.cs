using CleanArchTemplate.Domain.Security;

namespace CleanArchTemplate.Aplication.Features.Auth.Models.Input;

public sealed class TokenInput
{
    public required Guid User { get; set; }
    public required string Email { get; set; }
    public List<string> RoleNames { get; set; } = [];
    public List<string> PolicyNames { get; set; } = [];
}