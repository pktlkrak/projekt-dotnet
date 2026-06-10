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
    }
}
