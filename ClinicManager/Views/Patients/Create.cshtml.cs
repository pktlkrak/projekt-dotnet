using ClinicManager.Dtos.Patients;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Patients
{
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class CreateModel : PageModel
    {
        private readonly IPatientService _patientService;

        public CreateModel(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PatientFormDto Patient { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/Patients/Index";

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _patientService.CreatePatientAsync(Patient);

            return LocalRedirect(ReturnUrl);
        }
    }
}
