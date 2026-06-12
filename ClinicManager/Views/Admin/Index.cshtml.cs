using ClinicManager.Dtos.Medications;
using ClinicManager.Models;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminIndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMedicationService _medicationService;

        public AdminIndexModel(UserManager<ApplicationUser> userManager, IMedicationService medicationService)
        {
            _userManager = userManager;
            _medicationService = medicationService;
        }

        public List<UserWithRoles> PendingUsers { get; set; } = new();
        public List<UserWithRoles> Users { get; set; } = new();
        public List<MedicationDto> Medications { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            var allUsers = _userManager.Users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();

            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var entry = new UserWithRoles(u, roles.ToList());
                if (!u.EmailConfirmed)
                    PendingUsers.Add(entry);
                else
                    Users.Add(entry);
            }

            Medications = await _medicationService.GetAllMedicationsAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            StatusMessage = $"{user.LastName} {user.FirstName} approved.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);

            StatusMessage = "Registration rejected and account removed.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteMedicationAsync(int medicationId)
        {
            var deleted = await _medicationService.DeleteMedicationAsync(medicationId);
            if (deleted != null)
            {
                StatusMessage = $"Medication \"{deleted.Name}\" deleted.";
            }
            return RedirectToPage();
        }

        public record UserWithRoles(ApplicationUser User, List<string> Roles);
    }
}
