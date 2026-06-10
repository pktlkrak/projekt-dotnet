using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Views.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminIndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public AdminIndexModel(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<UserWithRoles> PendingUsers { get; set; } = new();
        public List<UserWithRoles> Users { get; set; } = new();
        public List<Medication> Medications { get; set; } = new();

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

            Medications = await _context.Medications.OrderBy(m => m.Name).ToListAsync();
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
            var med = await _context.Medications.FindAsync(medicationId);
            if (med != null)
            {
                _context.Medications.Remove(med);
                await _context.SaveChangesAsync();
                StatusMessage = $"Medication \"{med.Name}\" deleted.";
            }
            return RedirectToPage();
        }

        public record UserWithRoles(ApplicationUser User, List<string> Roles);
    }
}
