using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Medications
{
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class CreateMedicationModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateMedicationModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Medication Medication { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/Admin/Index";

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Medications.Add(Medication);
            await _context.SaveChangesAsync();

            return LocalRedirect(ReturnUrl);
        }
    }
}
