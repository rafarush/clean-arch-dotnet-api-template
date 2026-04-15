namespace CleanArchTemplate.Application.Services.Email.TemplateModels;

public sealed record ResetPasswordEmailModel(string UserName, string Link, int? Expires);