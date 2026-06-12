using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Procedures
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Procedure Procedure { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null) return NotFound();

            Procedure = procedure;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var existing = await _context.Procedures.FindAsync(Procedure.Id);
            if (existing == null) return NotFound();

            existing.Name = Procedure.Name;
            existing.Description = Procedure.Description;
            existing.Cost = Procedure.Cost;

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
