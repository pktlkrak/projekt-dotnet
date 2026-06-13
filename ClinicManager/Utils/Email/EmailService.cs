using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ClinicManager.Utils.Email;

public sealed record EmailAttachment(byte[] Data, string Filename, string MimeType);

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public string AdminAddress { get => _settings.FromAddress; }

    public async Task SendAsync(string to, string subject, string body, bool isHtml, params EmailAttachment[] attachments)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var builder = isHtml ? new BodyBuilder { HtmlBody = body } : new BodyBuilder { TextBody = body };
        foreach (var a in attachments)
            builder.Attachments.Add(a.Filename, a.Data, ContentType.Parse(a.MimeType));
        message.Body = builder.ToMessageBody();


        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(new SaslMechanismLogin(_settings.Username, _settings.Password));
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
