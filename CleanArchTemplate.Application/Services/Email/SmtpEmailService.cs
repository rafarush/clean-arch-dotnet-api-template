using CleanArchTemplate.Application.Services.Email.Abstractions;
using CleanArchTemplate.Application.Services.Email.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CleanArchTemplate.Application.Services.Email;

public class SmtpEmailService(
    IOptions<SmtpOptions> smtpOptions,
    ITemplateRenderer renderer) : IEmailService
{
    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        var mime = BuildMimeMessage(message, message.Body ?? string.Empty);
        await SendInternalAsync(mime, ct);
    }

    public async Task SendWithTemplateAsync<TModel>(EmailMessage message, string templateKey, TModel model,
        CancellationToken ct = default)
    {
        var html = await renderer.RenderAsync(templateKey, model);
        var mime = BuildMimeMessage(message, html);
        await SendInternalAsync(mime, ct);
    }
    
    private MimeMessage BuildMimeMessage(EmailMessage msg, string htmlBody)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(smtpOptions.Value.SenderName, smtpOptions.Value.SenderEmail));
        mime.To.Add(MailboxAddress.Parse(msg.To));
        msg.Cc?.ToList().ForEach(cc => mime.Cc.Add(MailboxAddress.Parse(cc)));
        mime.Subject = msg.Subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody };
        msg.Attachments?.ToList().ForEach(a =>
            builder.Attachments.Add(a.FileName, a.Content, ContentType.Parse(a.ContentType)));

        mime.Body = builder.ToMessageBody();
        return mime;
    }
    
    private async Task SendInternalAsync(MimeMessage mime, CancellationToken ct)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(smtpOptions.Value.Host, smtpOptions.Value.Port, smtpOptions.Value.UseSsl, ct);
        await client.AuthenticateAsync(smtpOptions.Value.Username, smtpOptions.Value.Password, ct);
        await client.SendAsync(mime, ct);
        await client.DisconnectAsync(true, ct);
    }
}