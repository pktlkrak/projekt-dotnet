using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (User.IsInRole("Admin"))
                return RedirectToPage("/Admin/Index");
            if (User.IsInRole("Doctor"))
                return RedirectToPage("/Doctor/Index");
            if (User.IsInRole("RegistrationWorker"))
                return RedirectToPage("/Registrar/Index");

            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
    }
}
