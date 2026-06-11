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
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class DetailsModel : PageModel
    {
        private readonly ClinicManager.Data.AppDbContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(ClinicManager.Data.AppDbContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Patient Patient { get; set; } = default!;

        public List<Visit> Visits { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/Patients/Index";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(m => m.Id == id);

            if (patient is not null)
            {
                Patient = patient;

                Visits = await _context.Visits
                    .Include(v => v.Doctor)
                    .Where(v => v.PatientId == id)
                    .OrderByDescending(v => v.ScheduledAt)
                    .ToListAsync();

                return Page();
            }

            _logger.LogWarning("Details requested for non-existent patient {PatientId}", id);
            return NotFound();
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

            return LocalRedirect(ReturnUrl);
        }
    }
}
