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

    public class IndexModel : PageModel
    {
        private readonly ClinicManager.Data.AppDbContext _context;

        public IndexModel(ClinicManager.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Patient> Patient { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public bool ShowDeleted { get; set; } = false;

        public async Task OnGetAsync()
        {
            // Show only non-deleted users by default, or all if toggle is on
            Patient = await _context.Patients
                .Where(p => !ShowDeleted ? !p.IsDeleted : true)
                .ToListAsync();
        }
    }
}
