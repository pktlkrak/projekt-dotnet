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
    public class IndexModel : PageModel
    {
        private readonly ClinicManager.Data.AppDbContext _context;

        public IndexModel(ClinicManager.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Patient> Patient { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Patient = await _context.Patients.ToListAsync();
        }
    }
}
