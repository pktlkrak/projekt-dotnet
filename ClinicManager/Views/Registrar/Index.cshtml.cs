using ClinicManager.Data;
using ClinicManager.Dtos.Patients;
using ClinicManager.Models;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Views.Registrar
{
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class RegistrarIndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IPatientService _patientService;

        public RegistrarIndexModel(AppDbContext context, IPatientService patientService)
        {
            _context = context;
            _patientService = patientService;
        }

        public IList<PatientDto> Patients { get; set; } = new List<PatientDto>();
        public List<Medication> Medications { get; set; } = new();

        public string? Search { get; set; }

        public async Task OnGetAsync(string? search = null)
        {
            Search = search;

            Patients = await _patientService.GetActivePatientsAsync(search);
            Medications = await _context.Medications.OrderBy(m => m.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteMedicationAsync(int medicationId)
        {
            var med = await _context.Medications.FindAsync(medicationId);
            if (med != null)
            {
                _context.Medications.Remove(med);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}