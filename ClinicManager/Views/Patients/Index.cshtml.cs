using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManager.Views.Patients
{
    [Authorize(Roles = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly ClinicManager.Data.AppDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ClinicManager.Data.AppDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Patient> Patient { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public bool ShowDeleted { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        public async Task OnGetAsync()
        {
            // Start with base query
            var query = _context.Patients.AsQueryable();

            // Filter by deleted status
            query = query.Where(p => !ShowDeleted ? !p.IsDeleted : true);

            // Search by PESEL or Surname
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(p =>
                    p.Pesel.Contains(SearchQuery) ||
                    p.LastName.Contains(SearchQuery));
            }

            Patient = await query.ToListAsync();

            _logger.LogInformation("Patient list loaded: {Count} records (showDeleted={ShowDeleted}, search={Search})",
                Patient.Count, ShowDeleted, SearchQuery);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                patient.IsDeleted = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Patient {PatientId} ({LastName}) soft-deleted", patient.Id, patient.LastName);
            }

            return RedirectToPage("./Index", new { ShowDeleted, SearchQuery });
        }

        public async Task<IActionResult> OnPostHardDeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                var visits = await _context.Visits.Where(v => v.PatientId == id).ToListAsync();
                var visitIds = visits.Select(v => (int?)v.Id).ToList();

                var procedures = await _context.Procedures
                    .Where(p => visitIds.Contains(EF.Property<int?>(p, "VisitId")))
                    .ToListAsync();

                var perscriptions = await _context.Perscriptions
                    .Where(p => visitIds.Contains(EF.Property<int?>(p, "VisitId")))
                    .ToListAsync();

                var perscriptionIds = perscriptions.Select(p => (int?)p.Id).ToList();
                var perscriptionItems = await _context.PerscriptionItems
                    .Where(pi => perscriptionIds.Contains(EF.Property<int?>(pi, "PerscriptionId")))
                    .ToListAsync();

                _context.PerscriptionItems.RemoveRange(perscriptionItems);
                _context.Perscriptions.RemoveRange(perscriptions);
                _context.Procedures.RemoveRange(procedures);
                _context.Visits.RemoveRange(visits);
                _context.Patients.Remove(patient);

                try
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Patient {PatientId} ({LastName}) and all related records permanently deleted", patient.Id, patient.LastName);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogWarning(ex, "Cannot permanently delete patient {PatientId}", id);
                    TempData["ErrorMessage"] = "Cannot permanently delete this patient because of related records that could not be removed.";
                }
            }

            return RedirectToPage("./Index", new { ShowDeleted, SearchQuery });
        }
    }
}
