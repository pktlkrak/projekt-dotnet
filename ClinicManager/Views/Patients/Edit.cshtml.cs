using ClinicManager.Dtos.Patients;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Patients
{
    [Authorize(Roles = "Admin,RegistrationWorker")]

    public class EditModel : PageModel
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IPatientService patientService, ILogger<EditModel> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        [BindProperty]
        public PatientFormDto Patient { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/Patients/Index";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientService.GetPatientForEditAsync(id.Value);
            if (patient == null)
            {
                _logger.LogWarning("Edit requested for non-existent patient {PatientId}", id);
                return NotFound();
            }

            Patient = patient;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updated = await _patientService.UpdatePatientAsync(Patient.Id, Patient);
            if (!updated)
            {
                _logger.LogWarning("Edit post for non-existent patient {PatientId}", Patient.Id);
                return NotFound();
            }

            return LocalRedirect(ReturnUrl);
        }
    }
}
