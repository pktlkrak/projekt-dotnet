using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace ClinicManager.Pages.Debug.Test;

public class AccessModel : PageModel
{
    public bool IsAuthenticated { get; private set; }
    public string? UserName { get; private set; }
    public string? Email { get; private set; }
    public IList<string> Roles { get; private set; } = [];

    public void OnGet()
    {
        IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
        UserName = User.Identity?.Name;
        Email = User.FindFirstValue(ClaimTypes.Email);
        Roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
    }
}
