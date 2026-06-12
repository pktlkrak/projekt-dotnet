namespace ClinicManager.Utils.Email;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, bool isHtml, params EmailAttachment[] attachments);
}
