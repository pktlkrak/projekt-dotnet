using ClinicManager.Dtos.Procedures;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Procedures
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IProcedureService _procedureService;

        public IndexModel(IProcedureService procedureService)
        {
            _procedureService = procedureService;
        }

        public IList<ProcedureDto> Procedures { get; set; } = [];

        [BindProperty]
        public ProcedureFormDto NewProcedure { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            Procedures = await _procedureService.GetAllProceduresAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                Procedures = await _procedureService.GetAllProceduresAsync();
                return Page();
            }

            await _procedureService.CreateProcedureAsync(NewProcedure);

            StatusMessage = $"Procedure \"{NewProcedure.Name}\" created.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var deleted = await _procedureService.DeleteProcedureAsync(id);
            if (deleted != null)
            {
                StatusMessage = $"Procedure \"{deleted.Name}\" deleted.";
            }

            return RedirectToPage();
        }
    }
}
