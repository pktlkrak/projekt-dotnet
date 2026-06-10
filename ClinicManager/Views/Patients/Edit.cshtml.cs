using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManager.Views.Patients
{
    [Authorize(Roles = "Admin,RegistrationWorker")]

    public class EditModel : PageModel
    {
        private readonly ClinicManager.Data.AppDbContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ClinicManager.Data.AppDbContext context, ILogger<EditModel> logger)
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

            var patient =  await _context.Patients.FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                _logger.LogWarning("Edit requested for non-existent patient {PatientId}", id);
                return NotFound();
            }
            Patient = patient;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the existing patient from database to preserve IsDeleted status
            var existingPatient = await _context.Patients.FindAsync(Patient.Id);
            if (existingPatient == null)
            {
                _logger.LogWarning("Edit post for non-existent patient {PatientId}", Patient.Id);
                return NotFound();
            }

            // Update only allowed properties, preserve IsDeleted
            existingPatient.FirstName = Patient.FirstName;
            existingPatient.LastName = Patient.LastName;
            existingPatient.Pesel = Patient.Pesel;
            existingPatient.PhoneNumber = Patient.PhoneNumber;
            existingPatient.Email = Patient.Email;
            existingPatient.DateOfBirth = Patient.DateOfBirth;
            existingPatient.InsuranceNumber = Patient.InsuranceNumber;
            // IsDeleted is NOT updated - can only be changed via Delete operation

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Patient {PatientId} ({LastName}) updated", existingPatient.Id, existingPatient.LastName);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PatientExists(Patient.Id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency conflict updating patient {PatientId}", Patient.Id);
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
