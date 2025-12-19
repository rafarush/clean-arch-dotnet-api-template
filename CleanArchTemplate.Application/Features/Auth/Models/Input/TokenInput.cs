using CleanArchTemplate.Domain.Security;

namespace CleanArchTemplate.Aplication.Features.Auth.Models.Input;

public sealed class TokenInput
{
    public required Guid User { get; set; }
    public required string Email { get; set; }
    public required IEnumerable<Policy> Policies { get; set; }
    public required IEnumerable<Role> Roles { get; set; }
}