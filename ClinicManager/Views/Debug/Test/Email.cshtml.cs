using ClinicManager.Utils.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ClinicManager.Pages.Debug.Test;

public class EmailModel(IEmailService emailService, IOptions<SmtpSettings> smtp) : PageModel
{
    [BindProperty]
    public string To { get; set; } = string.Empty;

    [BindProperty]
    public string Subject { get; set; } = "Test email";

    [BindProperty]
    public string Body { get; set; } = "This is a test email from ClinicManager.";

    [BindProperty]
    public bool IsHtml { get; set; }

    public string? Message { get; private set; }
    public bool IsError { get; private set; }

    public void OnGet()
    {
        To = smtp.Value.FromAddress;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await emailService.SendAsync(To, Subject, Body, IsHtml);
            Message = $"Email sent to {To}.";
        }
        catch (Exception ex)
        {
            IsError = true;
            Message = ex.Message;
        }

        return Page();
    }
}
