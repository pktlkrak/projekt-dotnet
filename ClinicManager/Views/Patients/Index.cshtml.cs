using ClinicManager.Dtos.Patients;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Patients
{
    [Authorize(Roles = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly IPatientAdminService _patientAdminService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IPatientAdminService patientAdminService, ILogger<IndexModel> logger)
        {
            _patientAdminService = patientAdminService;
            _logger = logger;
        }

        public IList<PatientDto> Patient { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public bool ShowDeleted { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        public async Task OnGetAsync()
        {
            Patient = await _patientAdminService.GetAllPatientsAsync(ShowDeleted, SearchQuery);

            _logger.LogInformation("Patient list loaded: {Count} records (showDeleted={ShowDeleted}, search={Search})",
                Patient.Count, ShowDeleted, SearchQuery);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _patientAdminService.SoftDeletePatientAsync(id);

            return RedirectToPage("./Index", new { ShowDeleted, SearchQuery });
        }

        public async Task<IActionResult> OnPostHardDeleteAsync(int id)
        {
            var result = await _patientAdminService.HardDeletePatientAsync(id);
            if (!result.Success && result.ErrorMessage != null)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToPage("./Index", new { ShowDeleted, SearchQuery });
        }
    }
}