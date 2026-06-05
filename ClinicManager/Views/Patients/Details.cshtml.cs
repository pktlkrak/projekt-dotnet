using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClinicManager.Data;
using ClinicManager.Models;

namespace ClinicManager.Views.Patients
{
    public class DetailsModel : PageModel
    {
        private readonly ClinicManager.Data.AppDbContext _context;

        public DetailsModel(ClinicManager.Data.AppDbContext context)
        {
            _context = context;
        }

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

            return NotFound();
        }
    }
}
