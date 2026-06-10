using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditUserModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EditUserModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; } = "";

        [BindProperty]
        public string NewRole { get; set; } = "";

        [BindProperty]
        public string? NewPassword { get; set; }

        public ApplicationUser EditUser { get; set; } = default!;
        public List<string> CurrentRoles { get; set; } = new();
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            EditUser = user;
            CurrentRoles = (await _userManager.GetRolesAsync(user)).ToList();
            NewRole = CurrentRoles.FirstOrDefault() ?? "Doctor";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return NotFound();

            EditUser = user;
            CurrentRoles = (await _userManager.GetRolesAsync(user)).ToList();

            var allowedRoles = new[] { "Admin", "Doctor", "RegistrationWorker" };
            if (!allowedRoles.Contains(NewRole))
            {
                StatusMessage = "Error: invalid role.";
                return Page();
            }

            // Update role
            if (!CurrentRoles.Contains(NewRole))
            {
                await _userManager.RemoveFromRolesAsync(user, CurrentRoles);
                var roleResult = await _userManager.AddToRoleAsync(user, NewRole);
                if (!roleResult.Succeeded)
                {
                    StatusMessage = "Error: " + string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return Page();
                }
                CurrentRoles = new List<string> { NewRole };
            }

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var pwResult = await _userManager.ResetPasswordAsync(user, token, NewPassword);
                if (!pwResult.Succeeded)
                {
                    StatusMessage = "Error: " + string.Join(", ", pwResult.Errors.Select(e => e.Description));
                    return Page();
                }
            }

            StatusMessage = "User updated successfully.";
            return Page();
        }
    }
}
