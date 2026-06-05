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

        public EditModel(ClinicManager.Data.AppDbContext context)
        {
            _context = context;
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
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(Patient.Id))
                {
                    return NotFound();
                }
                else
                {
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
