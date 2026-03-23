namespace CleanArchTemplate.Application.Services.Email.Options;

public class ScribanOptions
{
    public const string Section = "Email:Templates";
    public string TemplatesPath { get; init; } = "Services/Email/Templates";
}