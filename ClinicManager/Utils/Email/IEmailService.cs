namespace ClinicManager.Utils.Email;

public interface IEmailService
{
    string AdminAddress { get; }
    Task SendAsync(string to, string subject, string body, bool isHtml, params EmailAttachment[] attachments);
}
