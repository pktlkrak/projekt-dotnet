using ClinicManager.Dtos.Procedures;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Procedures
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IProcedureService _procedureService;

        public EditModel(IProcedureService procedureService)
        {
            _procedureService = procedureService;
        }

        [BindProperty]
        public ProcedureFormDto Procedure { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var dto = await _procedureService.GetProcedureForEditAsync(id.Value);
            if (dto == null) return NotFound();

            Procedure = dto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var success = await _procedureService.UpdateProcedureAsync(Procedure.Id, Procedure);
            if (!success) return NotFound();

            return RedirectToPage("Index");
        }
    }
}
