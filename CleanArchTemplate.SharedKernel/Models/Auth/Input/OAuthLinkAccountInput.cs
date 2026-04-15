using CleanArchTemplate.Domain.AuthProvider;

namespace CleanArchTemplate.SharedKernel.Models.Auth.Input;

public class OAuthLinkAccountInput
{
    public required Guid UserId { get; set; }
    public required OAuthProviderType Provider { get; set; }
    public required string Code { get; set; }
}
