using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Admin
{
    [Authorize]
    public class ImpersonateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ImpersonateModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostStartAsync(string userId)
        {
            if (!User.IsInRole("Admin")) return Forbid();

            var target = await _userManager.FindByIdAsync(userId);
            if (target == null) return NotFound();

            var originalAdminId = _userManager.GetUserId(User)!;
            HttpContext.Session.SetString("OriginalAdminId", originalAdminId);

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(target, isPersistent: false);

            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostStopAsync()
        {
            var originalAdminId = HttpContext.Session.GetString("OriginalAdminId");
            if (string.IsNullOrEmpty(originalAdminId))
                return RedirectToPage("/Index");

            var admin = await _userManager.FindByIdAsync(originalAdminId);
            if (admin == null) return RedirectToPage("/Index");

            HttpContext.Session.Remove("OriginalAdminId");

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(admin, isPersistent: false);

            return RedirectToPage("/Admin/Index");
        }
    }
}
