using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Views.Visits
{
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Visit Visit { get; set; } = new() { ScheduledAt = DateTime.Now.AddHours(1) };

        public List<SelectListItem> PatientItems { get; set; } = new();
        public List<SelectListItem> DoctorItems { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string? doctorId = null, int? patientId = null)
        {
            await LoadSelectsAsync();

            if (doctorId != null)
                Visit.DoctorId = doctorId;
            if (patientId != null)
                Visit.PatientId = patientId.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Visit.Survey = "";
            Visit.Diagnosis = "";
            Visit.Recommendations = "";

            ModelState.Remove(nameof(Visit.Survey));
            ModelState.Remove(nameof(Visit.Diagnosis));
            ModelState.Remove(nameof(Visit.Recommendations));

            if (!ModelState.IsValid)
            {
                await LoadSelectsAsync();
                return Page();
            }

            _context.Visits.Add(Visit);
            await _context.SaveChangesAsync();

            return RedirectToPage("/", new { doctorId = Visit.DoctorId });
        }

        private async Task LoadSelectsAsync()
        {
            var patients = await _context.Patients
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.LastName)
                .ToListAsync();

            PatientItems = patients
                .Select(p => new SelectListItem($"{p.LastName} {p.FirstName} ({p.Pesel})", p.Id.ToString()))
                .ToList();

            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            DoctorItems = doctors
                .OrderBy(d => d.LastName)
                .Select(d => new SelectListItem($"{d.LastName} {d.FirstName}", d.Id))
                .ToList();
        }
    }
}
