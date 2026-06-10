using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Medications
{
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class EditMedicationModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditMedicationModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Medication Medication { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/Admin/Index";

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var med = await _context.Medications.FindAsync(id);
            if (med == null) return NotFound();
            Medication = med;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var existing = await _context.Medications.FindAsync(Medication.Id);
            if (existing == null) return NotFound();

            existing.Name = Medication.Name;
            existing.Cost = Medication.Cost;
            await _context.SaveChangesAsync();

            return LocalRedirect(ReturnUrl);
        }
    }
}
