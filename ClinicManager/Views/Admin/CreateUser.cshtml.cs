using System.ComponentModel.DataAnnotations;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateUserModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            public string FirstName { get; set; } = "";

            [Required]
            public string LastName { get; set; } = "";

            [Required, EmailAddress]
            public string Email { get; set; } = "";

            [Required, MinLength(6)]
            public string Password { get; set; } = "";

            [Required]
            public string Role { get; set; } = "Admin";
        }

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var allowedRoles = new[] { "Admin", "Doctor", "RegistrationWorker" };
            if (!allowedRoles.Contains(Input.Role))
            {
                ModelState.AddModelError("", "Invalid role.");
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                return Page();
            }

            await _userManager.AddToRoleAsync(user, Input.Role);

            TempData["StatusMessage"] = $"User {user.Email} created with role {Input.Role}.";
            return RedirectToPage("/Admin/Index");
        }
    }
}
