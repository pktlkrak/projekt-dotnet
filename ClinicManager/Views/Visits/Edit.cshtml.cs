using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Views.Visits
{
    [Authorize(Roles = "Admin,Doctor,RegistrationWorker")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Visit Visit { get; set; } = default!;

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var visit = await _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visit == null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var userId = _userManager.GetUserId(User);
                if (visit.DoctorId != userId) return Forbid();
            }

            Visit = visit;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existing = await _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .FirstOrDefaultAsync(v => v.Id == Visit.Id);

            if (existing == null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var userId = _userManager.GetUserId(User);
                if (existing.DoctorId != userId) return Forbid();
            }

            existing.Status = Visit.Status;

            if (User.IsInRole("Doctor"))
            {
                existing.Survey = Visit.Survey ?? "";
                existing.Diagnosis = Visit.Diagnosis ?? "";
                existing.Recommendations = Visit.Recommendations ?? "";
            }

            if (!User.IsInRole("Doctor"))
                existing.ScheduledAt = Visit.ScheduledAt;

            await _context.SaveChangesAsync();

            StatusMessage = "Appointment saved.";

            Visit = existing;
            return Page();
        }
    }
}
