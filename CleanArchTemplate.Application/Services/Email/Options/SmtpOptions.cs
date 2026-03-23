namespace CleanArchTemplate.Application.Services.Email.Options;

public class SmtpOptions
{
    public const string Section = "Email:Smtp";
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required bool UseSsl { get; init; } = true;
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string SenderEmail { get; init; }
    public required string SenderName { get; init; }
}