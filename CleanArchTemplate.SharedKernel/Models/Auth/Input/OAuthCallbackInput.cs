namespace CleanArchTemplate.SharedKernel.Models.Auth.Input;

public class OAuthCallbackInput
{
    public required string Provider { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public string? LastName { get; set; }
    public required string ProviderId { get; set; }
}
