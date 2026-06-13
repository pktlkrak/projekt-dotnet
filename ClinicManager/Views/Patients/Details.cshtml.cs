using ClinicManager.Dtos.MedicalFiles;
using ClinicManager.Dtos.Patients;
using ClinicManager.Dtos.Visits;
using ClinicManager.Services;
using ClinicManager.Utils.PDF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Patients
{
    [Authorize(Roles = "Admin,RegistrationWorker,Doctor")]
    public class DetailsModel : PageModel
    {
        private readonly IPatientService _patientService;
        private readonly IVisitService _visitService;
        private readonly IMedicalFileService _medicalFileService;
        private readonly IReportService _reportService;
        private readonly ILogger<DetailsModel> _logger;
        private readonly IWebHostEnvironment _env;

        public DetailsModel(IPatientService patientService, IVisitService visitService, IMedicalFileService medicalFileService, IReportService reportService, ILogger<DetailsModel> logger, IWebHostEnvironment env)
        {
            _patientService = patientService;
            _visitService = visitService;
            _medicalFileService = medicalFileService;
            _reportService = reportService;
            _logger = logger;
            _env = env;
        }

        public PatientDto Patient { get; set; } = default!;

        public List<VisitDto> Visits { get; set; } = new();

        public List<MedicalFileDto> MedicalFiles { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/Patients/Index";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientService.GetPatientAsync(id.Value);

            if (patient is not null)
            {
                Patient = patient;

                Visits = await _visitService.GetVisitsForPatientAsync(id.Value);
                MedicalFiles = await _medicalFileService.GetFilesForPatientAsync(id.Value);

                return Page();
            }

            _logger.LogWarning("Details requested for non-existent patient {PatientId}", id);
            return NotFound();
        }

        public async Task<IActionResult> OnGetPatientReportPdfAsync(int id)
        {
            if (!User.IsInRole("Admin")) return Forbid();

            var data = await _reportService.GetPatientReportAsync(id);
            if (data == null) return NotFound();

            var bytes = PdfAdminReportWriter.GeneratePatientReport(data);
            var filename = $"patient-report-{data.PatientName.Replace(" ", "-").ToLower()}-{data.Pesel}.pdf";
            return File(bytes, "application/pdf", filename);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _patientService.SoftDeletePatientAsync(id);

            return LocalRedirect(ReturnUrl);
        }

        public async Task<IActionResult> OnPostUploadAsync(int id, IFormFile? file)
        {
            var patient = await _patientService.GetPatientAsync(id);
            if (patient == null) return NotFound();

            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToPage("./Details", new { id, returnUrl = ReturnUrl });
            }

            if (string.IsNullOrEmpty(file.ContentType) || !file.ContentType.StartsWith("image/"))
            {
                TempData["ErrorMessage"] = "Only image files can be uploaded.";
                return RedirectToPage("./Details", new { id, returnUrl = ReturnUrl });
            }

            var folder = Path.Combine(_env.WebRootPath, "medicalDocuments", patient.Pesel);
            Directory.CreateDirectory(folder);

            var fileName = Path.GetFileName(file.FileName);
            var fullPath = Path.Combine(folder, fileName);
            if (System.IO.File.Exists(fullPath))
            {
                fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
                fullPath = Path.Combine(folder, fileName);
            }

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _medicalFileService.AddFileAsync(patient.Id, $"medicalDocuments/{patient.Pesel}/{fileName}");

            _logger.LogInformation("Uploaded document {FileName} for patient {PatientId}", fileName, patient.Id);

            return RedirectToPage("./Details", new { id, returnUrl = ReturnUrl });
        }

        public async Task<IActionResult> OnPostDeleteFileAsync(int id, int fileId)
        {
            var file = await _medicalFileService.GetFileAsync(fileId);
            if (file != null && file.PatientId == id)
            {
                var fullPath = Path.Combine(_env.WebRootPath, file.Path);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                await _medicalFileService.DeleteFileAsync(fileId);

                _logger.LogInformation("Deleted document {FileId} for patient {PatientId}", fileId, id);
            }

            return RedirectToPage("./Details", new { id, returnUrl = ReturnUrl });
        }
    }
}
