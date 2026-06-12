using ClinicManager.Dtos.Patients;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Patients
{
    [Authorize(Roles = "Admin")]

    public class DeleteModel : PageModel
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IPatientService patientService, ILogger<DeleteModel> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        [BindProperty]
        public PatientDto Patient { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientService.GetPatientAsync(id.Value);

            if (patient is not null)
            {
                Patient = patient;

                return Page();
            }

            _logger.LogWarning("Delete requested for non-existent patient {PatientId}", id);
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _patientService.SoftDeletePatientAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}
