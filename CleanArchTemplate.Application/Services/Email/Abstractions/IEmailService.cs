namespace CleanArchTemplate.Application.Services.Email.Abstractions;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
    Task SendWithTemplateAsync<TModel>(EmailMessage message, string templateKey, TModel model, CancellationToken ct = default);
}