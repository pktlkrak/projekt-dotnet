using ClinicManager.Models;
using ClinicManager.Services;
using ClinicManager.Utils.PDF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicManager.Views.Reports;

[Authorize(Roles = "Admin")]
public class IndexModel(IReportService reportService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? DoctorId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Year { get; set; } = DateTime.Today.Year;

    [BindProperty(SupportsGet = true)]
    public int Month { get; set; } = DateTime.Today.Month;

    public List<SelectListItem> DoctorItems { get; set; } = [];

    public async Task OnGetAsync()
    {
        var doctors = await reportService.GetDoctorsAsync();
        DoctorItems = doctors.Select(d => new SelectListItem($"{d.LastName} {d.FirstName}", d.Id)).ToList();
    }

    public async Task<IActionResult> OnGetPdfAsync()
    {
        if (string.IsNullOrEmpty(DoctorId) || Year < 2000 || Month < 1 || Month > 12)
            return BadRequest();

        var data = await reportService.GetMonthlyReportAsync(Year, Month, DoctorId);
        if (data == null) return NotFound();

        var bytes = PdfAdminReportWriter.GenerateMonthlyReport(data);
        var monthName = new DateTime(Year, Month, 1).ToString("yyyy-MM");
        var filename = $"report-monthly-{monthName}-{data.DoctorName.Replace(" ", "-").ToLower()}.pdf";
        return File(bytes, "application/pdf", filename);
    }
}
