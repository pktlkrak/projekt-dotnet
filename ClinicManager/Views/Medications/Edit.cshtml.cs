using ClinicManager.Dtos.Medications;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Medications
{
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class EditMedicationModel : PageModel
    {
        private readonly IMedicationService _medicationService;

        public EditMedicationModel(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        [BindProperty]
        public MedicationFormDto Medication { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/Admin/Index";

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dto = await _medicationService.GetMedicationForEditAsync(id);
            if (dto == null) return NotFound();

            Medication = dto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var success = await _medicationService.UpdateMedicationAsync(Medication.Id, Medication);
            if (!success) return NotFound();

            return LocalRedirect(ReturnUrl);
        }
    }
}
