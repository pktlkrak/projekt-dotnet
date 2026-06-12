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
    public class DetailsModel : PageModel
    {
        private readonly ClinicManager.Data.AppDbContext _context;
        private readonly ILogger<DetailsModel> _logger;
        private readonly IWebHostEnvironment _env;

        public DetailsModel(ClinicManager.Data.AppDbContext context, ILogger<DetailsModel> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        public Patient Patient { get; set; } = default!;

        public List<Visit> Visits { get; set; } = new();

        public List<MedicalFile> MedicalFiles { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = "/Patients/Index";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(m => m.Id == id);

            if (patient is not null)
            {
                Patient = patient;

                Visits = await _context.Visits
                    .Include(v => v.Doctor)
                    .Where(v => v.PatientId == id)
                    .OrderByDescending(v => v.ScheduledAt)
                    .ToListAsync();

                MedicalFiles = await _context.MedicalFiles
                    .Where(m => m.PatientId == id)
                    .ToListAsync();

                return Page();
            }

            _logger.LogWarning("Details requested for non-existent patient {PatientId}", id);
            return NotFound();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                patient.IsDeleted = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Patient {PatientId} ({LastName}) soft-deleted", patient.Id, patient.LastName);
            }

            return LocalRedirect(ReturnUrl);
        }

        public async Task<IActionResult> OnPostUploadAsync(int id, IFormFile? file)
        {
            var patient = await _context.Patients.FindAsync(id);
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

            _context.MedicalFiles.Add(new MedicalFile
            {
                PatientId = patient.Id,
                Path = $"medicalDocuments/{patient.Pesel}/{fileName}"
            });
            await _context.SaveChangesAsync();

            _logger.LogInformation("Uploaded document {FileName} for patient {PatientId}", fileName, patient.Id);

            return RedirectToPage("./Details", new { id, returnUrl = ReturnUrl });
        }

        public async Task<IActionResult> OnPostDeleteFileAsync(int id, int fileId)
        {
            var file = await _context.MedicalFiles.FindAsync(fileId);
            if (file != null && file.PatientId == id)
            {
                var fullPath = Path.Combine(_env.WebRootPath, file.Path);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                _context.MedicalFiles.Remove(file);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted document {FileId} for patient {PatientId}", fileId, id);
            }

            return RedirectToPage("./Details", new { id, returnUrl = ReturnUrl });
        }
    }
}
