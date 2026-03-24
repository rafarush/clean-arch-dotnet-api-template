namespace CleanArchTemplate.Application.Services.Email.TemplateModels;

public sealed record ConfirmEmailModel(string UserName, string Link, int? Expires);