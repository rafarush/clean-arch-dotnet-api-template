namespace CleanArchTemplate.SharedKernel.Models.Auth.Input;

public class ResetPasswordInput
{
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}