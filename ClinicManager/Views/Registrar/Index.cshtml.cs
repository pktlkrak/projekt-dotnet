using ClinicManager.Dtos.Medications;
using ClinicManager.Dtos.Patients;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Registrar
{
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class RegistrarIndexModel : PageModel
    {
        private readonly IPatientService _patientService;
        private readonly IMedicationService _medicationService;

        public RegistrarIndexModel(IPatientService patientService, IMedicationService medicationService)
        {
            _patientService = patientService;
            _medicationService = medicationService;
        }

        public IList<PatientDto> Patients { get; set; } = new List<PatientDto>();
        public List<MedicationDto> Medications { get; set; } = new();

        public string? Search { get; set; }

        public async Task OnGetAsync(string? search = null)
        {
            Search = search;

            Patients = await _patientService.GetActivePatientsAsync(search);
            Medications = await _medicationService.GetAllMedicationsAsync();
        }

        public async Task<IActionResult> OnPostDeleteMedicationAsync(int medicationId)
        {
            await _medicationService.DeleteMedicationAsync(medicationId);
            return RedirectToPage();
        }
    }
}