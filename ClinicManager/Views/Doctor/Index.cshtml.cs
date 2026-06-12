using ClinicManager.Dtos.Visits;
using ClinicManager.Models;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class DoctorIndexModel : PageModel
    {
        private readonly IVisitService _visitService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorIndexModel(IVisitService visitService, UserManager<ApplicationUser> userManager)
        {
            _visitService = visitService;
            _userManager = userManager;
        }

        public List<VisitDto> TodayVisits { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var doctorId = _userManager.GetUserId(User);
            if (doctorId == null) return RedirectToPage("/Index");

            TodayVisits = await _visitService.GetTodaysVisitsForDoctorAsync(doctorId);

            return Page();
        }
    }
}
