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

    public class DeleteModel : PageModel
    {
        private readonly ClinicManager.Data.AppDbContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(ClinicManager.Data.AppDbContext context, ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Patient Patient { get; set; } = default!;

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

            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                // Soft delete - set IsDeleted to true instead of removing
                patient.IsDeleted = true;
                _context.Patients.Update(patient);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Patient {PatientId} ({LastName}) soft-deleted", patient.Id, patient.LastName);
            }

            return RedirectToPage("./Index");
        }
    }
}
