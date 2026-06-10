using ClinicManager.Data;
using ClinicManager.Models;
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

        public RegistrarIndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Patient> Patients { get; set; } = new List<Patient>();
        public List<Medication> Medications { get; set; } = new();

        public string? Search { get; set; }

        public async Task OnGetAsync(string? search = null)
        {
            Search = search;
            var query = _context.Patients.Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.LastName.Contains(search) ||
                    p.FirstName.Contains(search) ||
                    p.Pesel.Contains(search));
            }

            Patients = await query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToListAsync();
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
