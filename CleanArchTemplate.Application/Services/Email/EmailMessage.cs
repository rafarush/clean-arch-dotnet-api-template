namespace CleanArchTemplate.Application.Services.Email;

public record EmailMessage(
    string To,
    string Subject,
    string? Body = null,
    bool IsHtml = true,
    IEnumerable<string>? Cc = null,
    IEnumerable<EmailAttachment>? Attachments = null
);

public record EmailAttachment(string FileName, byte[] Content, string ContentType);