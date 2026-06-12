using ClinicManager.Dtos.Medications;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Medications
{
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class CreateMedicationModel : PageModel
    {
        private readonly IMedicationService _medicationService;

        public CreateMedicationModel(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        [BindProperty]
        public MedicationFormDto Medication { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/Admin/Index";

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _medicationService.CreateMedicationAsync(Medication);

            return LocalRedirect(ReturnUrl);
        }
    }
}
