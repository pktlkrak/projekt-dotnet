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
        private readonly IProcedureService _procedureService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(IVisitService visitService, IPatientService patientService, IProcedureService procedureService, UserManager<ApplicationUser> userManager)
        {
            _visitService = visitService;
            _patientService = patientService;
            _procedureService = procedureService;
            _userManager = userManager;
        }

        [BindProperty]
        public VisitCreateDto Visit { get; set; } = new() { ScheduledAt = DateTime.Now.AddHours(1) };

        public List<SelectListItem> PatientItems { get; set; } = [];
        public List<SelectListItem> DoctorItems { get; set; } = [];
        public List<SelectListItem> ProcedureItems { get; set; } = [];
        public Dictionary<int, double> ProcedureCosts { get; set; } = [];

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
            if (!Visit.Procedures.Any())
                ModelState.AddModelError("Visit.Procedures", "At least one procedure is required.");

            if (ModelState.IsValid && !await _visitService.IsDoctorAvailableAsync(Visit.DoctorId, Visit.ScheduledAt))
                ModelState.AddModelError("Visit.ScheduledAt", "The doctor already has an appointment within 1.5 hours of this time.");

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

            var procedures = await _procedureService.GetAllProceduresAsync();
            ProcedureItems = procedures
                .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
                .ToList();
            ProcedureCosts = procedures.ToDictionary(p => p.Id, p => p.Cost);
        }
    }
}
