using ClinicManager.Dtos.Visits;
using ClinicManager.Models;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicManager.Views.Visits
{
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class CreateModel : PageModel
    {
        private readonly IVisitService _visitService;
        private readonly IPatientService _patientService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(IVisitService visitService, IPatientService patientService, UserManager<ApplicationUser> userManager)
        {
            _visitService = visitService;
            _patientService = patientService;
            _userManager = userManager;
        }

        [BindProperty]
        public VisitCreateDto Visit { get; set; } = new() { ScheduledAt = DateTime.Now.AddHours(1) };

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
            if (!ModelState.IsValid)
            {
                await LoadSelectsAsync();
                return Page();
            }

            await _visitService.CreateVisitAsync(Visit);

            return RedirectToPage("/Index", new { doctorId = Visit.DoctorId });
        }

        private async Task LoadSelectsAsync()
        {
            var patients = await _patientService.GetActivePatientsAsync(null);

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
