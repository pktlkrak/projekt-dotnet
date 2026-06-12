using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClinicManager.Data;
using ClinicManager.Models;

namespace ClinicManager.Views.Patients
{
    [Authorize(Roles = "Admin,RegistrationWorker")]
    public class CreateModel : PageModel
    {
        private readonly ClinicManager.Data.AppDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ClinicManager.Data.AppDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Patient Patient { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/Patients/Index";

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Always create non-deleted users
            Patient.IsDeleted = false;

            _context.Patients.Add(Patient);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Patient {PatientId} ({LastName}) created", Patient.Id, Patient.LastName);

            return LocalRedirect(ReturnUrl);
        }
    }
}
