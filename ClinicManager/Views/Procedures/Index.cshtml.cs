using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Views.Procedures
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Procedure> Procedures { get; set; } = [];

        [BindProperty]
        public Procedure NewProcedure { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            Procedures = await _context.Procedures.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                Procedures = await _context.Procedures.OrderBy(p => p.Name).ToListAsync();
                return Page();
            }

            _context.Procedures.Add(NewProcedure);
            await _context.SaveChangesAsync();

            StatusMessage = $"Procedure \"{NewProcedure.Name}\" created.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure != null)
            {
                _context.Procedures.Remove(procedure);
                await _context.SaveChangesAsync();
                StatusMessage = $"Procedure \"{procedure.Name}\" deleted.";
            }

            return RedirectToPage();
        }
    }
}
