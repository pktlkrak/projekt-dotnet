using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Views.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class DoctorIndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorIndexModel(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Visit> TodayVisits { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var doctorId = _userManager.GetUserId(User);
            if (doctorId == null) return RedirectToPage("/Index");

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            TodayVisits = await _context.Visits
                .Include(v => v.Patient)
                .Where(v => v.DoctorId == doctorId
                    && v.ScheduledAt >= today
                    && v.ScheduledAt < tomorrow)
                .OrderBy(v => v.ScheduledAt)
                .ToListAsync();

            return Page();
        }
    }
}
