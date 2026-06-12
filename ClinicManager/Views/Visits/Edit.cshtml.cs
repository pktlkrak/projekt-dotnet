using System.Globalization;
using System.Text;
using ClinicManager.Data;
using ClinicManager.Models;
using ClinicManager.Utils.PDF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Views.Visits
{
    [Authorize(Roles = "Admin,Doctor,RegistrationWorker")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Visit Visit { get; set; } = default!;

        [BindProperty]
        public PrescriptionInput NewPrescription { get; set; } = new();

        [BindProperty]
        public string ReturnUrl { get; set; } = "/Doctor/Index";

        [TempData]
        public string? StatusMessage { get; set; }

        public List<SelectListItem> MedicationItems { get; set; } = [];
        public Dictionary<int, double> MedicationCosts { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var visit = await LoadVisitAsync(id.Value);
            if (visit == null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var userId = _userManager.GetUserId(User);
                if (visit.DoctorId != userId) return Forbid();
            }

            Visit = visit;
            NewPrescription.VisitId = visit.Id;
            await LoadMedicationsAsync();

            var referer = Request.Headers.Referer.ToString();
            if (Uri.TryCreate(referer, UriKind.Absolute, out var refUri) && refUri.PathAndQuery.StartsWith('/'))
                ReturnUrl = refUri.PathAndQuery;

            return Page();
        }

        public async Task<IActionResult> OnGetPdfAsync(int? id)
        {
            if (id == null) return NotFound();

            var visit = await LoadVisitAsync(id.Value);
            if (visit == null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var userId = _userManager.GetUserId(User);
                if (visit.DoctorId != userId) return Forbid();
            }

            var patient = visit.Patient;
            var doctor = visit.Doctor;

            var patientInfo = new StringBuilder();
            if (patient != null)
            {
                patientInfo.AppendLine($"{patient.LastName} {patient.FirstName}");
                patientInfo.AppendLine($"PESEL: {patient.Pesel}");
                patientInfo.AppendLine($"DOB: {patient.DateOfBirth:d MMM yyyy}");
                patientInfo.AppendLine($"Phone: {patient.PhoneNumber}");
                patientInfo.Append($"Insurance: {patient.InsuranceNumber}");
            }

            var visitInfo = new StringBuilder();
            visitInfo.AppendLine($"Date: {visit.ScheduledAt:d MMM yyyy HH:mm}");
            visitInfo.Append($"Doctor: {doctor?.LastName} {doctor?.FirstName}");

            var prescriptionSections = visit.Reciepts.Select((rx, i) =>
            {
                var sb = new StringBuilder();
                foreach (var item in rx.PerscriptionItem)
                    sb.AppendLine($"• {item.Medication?.Name}  |  {item.Dosage}  |  {item.Amount}  |  {item.Price:C}");
                sb.Append($"Total: {rx.PerscriptionItem.Sum(it => it.Price):C}");
                return new PdfSection($"Prescription #{i + 1}", sb.ToString());
            }).ToArray();

            var data = new PdfReportData(
                $"Visit Report — {patient?.LastName} {patient?.FirstName}",
                $"{visit.ScheduledAt:d MMM yyyy}  ·  Dr. {doctor?.LastName} {doctor?.FirstName}"
            )
            {
                TopLeft = [new("Patient", patientInfo.ToString().TrimEnd())],
                TopRight = [new("Visit", visitInfo.ToString().TrimEnd())],
                Middle =
                [
                    new("Interview", string.IsNullOrWhiteSpace(visit.Survey) ? "—" : visit.Survey),
                    new("Diagnosis", string.IsNullOrWhiteSpace(visit.Diagnosis) ? "—" : visit.Diagnosis),
                    new("Recommendations", string.IsNullOrWhiteSpace(visit.Recommendations) ? "—" : visit.Recommendations),
                ],
                BottomLeft = prescriptionSections.Length > 0 ? prescriptionSections : [new("Prescriptions", "None")],
            };

            var bytes = PdfReportWriter.GenerateBytes(data);
            var filename = $"visit-{visit.Id}-{patient?.LastName?.ToLower()}.pdf";
            return File(bytes, "application/pdf", filename);
        }

        public async Task<IActionResult> OnGetPrescriptionPdfAsync(int? id, int prescriptionId)
        {
            if (id == null) return NotFound();

            var visit = await LoadVisitAsync(id.Value);
            if (visit == null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var userId = _userManager.GetUserId(User);
                if (visit.DoctorId != userId) return Forbid();
            }

            var rx = visit.Reciepts
                .Select((r, i) => (r, i))
                .FirstOrDefault(t => t.r.Id == prescriptionId);

            if (rx.r == null) return NotFound();

            var patient = visit.Patient;
            var doctor = visit.Doctor;
            var index = rx.i + 1;

            var patientInfo = new StringBuilder();
            if (patient != null)
            {
                patientInfo.AppendLine($"{patient.LastName} {patient.FirstName}");
                patientInfo.AppendLine($"PESEL: {patient.Pesel}");
                patientInfo.Append($"DOB: {patient.DateOfBirth:d MMM yyyy}");
            }

            var visitInfo = new StringBuilder();
            visitInfo.AppendLine($"Date: {visit.ScheduledAt:d MMM yyyy HH:mm}");
            visitInfo.Append($"Doctor: {doctor?.LastName} {doctor?.FirstName}");

            var itemsText = new StringBuilder();
            foreach (var item in rx.r.PerscriptionItem)
                itemsText.AppendLine($"• {item.Medication?.Name}  |  {item.Dosage}  |  {item.Amount}  |  {item.Price:C}");
            itemsText.Append($"Total: {rx.r.PerscriptionItem.Sum(it => it.Price):C}");

            var data = new PdfReportData(
                $"Prescription #{index} — {patient?.LastName} {patient?.FirstName}",
                $"{visit.ScheduledAt:d MMM yyyy}  ·  Dr. {doctor?.LastName} {doctor?.FirstName}"
            )
            {
                TopLeft = [new("Patient", patientInfo.ToString().TrimEnd())],
                TopRight = [new("Visit", visitInfo.ToString().TrimEnd())],
                BottomLeft = [new("Medications", itemsText.ToString().TrimEnd())],
            };

            var bytes = PdfReportWriter.GenerateBytes(data);
            var filename = $"prescription-{index}-visit-{visit.Id}-{patient?.LastName?.ToLower()}.pdf";
            return File(bytes, "application/pdf", filename);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existing = await LoadVisitAsync(Visit.Id);
            if (existing == null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var userId = _userManager.GetUserId(User);
                if (existing.DoctorId != userId) return Forbid();
            }

            existing.Status = Visit.Status;

            if (User.IsInRole("Doctor"))
            {
                existing.Survey = Visit.Survey ?? "";
                existing.Diagnosis = Visit.Diagnosis ?? "";
                existing.Recommendations = Visit.Recommendations ?? "";
            }

            if (!User.IsInRole("Doctor"))
                existing.ScheduledAt = Visit.ScheduledAt;

            await _context.SaveChangesAsync();

            StatusMessage = "Appointment saved.";
            Visit = existing;
            await LoadMedicationsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostFinishAsync()
        {
            var existing = await _context.Visits.FirstOrDefaultAsync(v => v.Id == Visit.Id);
            if (existing == null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var userId = _userManager.GetUserId(User);
                if (existing.DoctorId != userId) return Forbid();
            }

            existing.Status = VisitStatus.Finished;
            existing.Survey = Visit.Survey ?? "";
            existing.Diagnosis = Visit.Diagnosis ?? "";
            existing.Recommendations = Visit.Recommendations ?? "";

            await _context.SaveChangesAsync();

            var url = ReturnUrl.StartsWith('/') ? ReturnUrl : "/Doctor/Index";
            return LocalRedirect(url);
        }

        public async Task<IActionResult> OnPostAddPrescriptionAsync()
        {
            var doctorId = _userManager.GetUserId(User);
            var visit = await LoadVisitAsync(NewPrescription.VisitId);

            if (visit == null) return NotFound();
            if (visit.DoctorId != doctorId) return Forbid();

            var validItems = NewPrescription.Items?.Where(i => i.MedicationId > 0).ToList() ?? [];
            if (validItems.Count == 0)
            {
                ModelState.AddModelError("", "At least one medication is required.");
                Visit = visit;
                await LoadMedicationsAsync();
                return Page();
            }

            var prescription = new Perscription
            {
                VisitId = NewPrescription.VisitId,
                Description = NewPrescription.Description,
                PerscriptionItem = validItems.Select(i => new PerscriptionItem
                {
                    MedicationId = i.MedicationId,
                    Dosage = i.Dosage,
                    Amount = i.Amount,
                    Price = double.TryParse(i.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var p) ? p : 0
                }).ToList()
            };

            _context.Perscriptions.Add(prescription);
            await _context.SaveChangesAsync();

            StatusMessage = "Prescription added.";
            return RedirectToPage(new { id = NewPrescription.VisitId });
        }

        public async Task<IActionResult> OnPostDeletePrescriptionAsync(int prescriptionId)
        {
            var prescription = await _context.Perscriptions
                .Include(p => p.PerscriptionItem)
                .FirstOrDefaultAsync(p => p.Id == prescriptionId);

            if (prescription == null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var doctorId = _userManager.GetUserId(User);
                var visit = await _context.Visits.FindAsync(prescription.VisitId);
                if (visit == null || visit.DoctorId != doctorId) return Forbid();
            }

            _context.Perscriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            StatusMessage = "Prescription deleted.";
            return RedirectToPage(new { id = prescription.VisitId });
        }

        private Task<Visit?> LoadVisitAsync(int id) =>
            _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Include(v => v.Reciepts)
                    .ThenInclude(p => p.PerscriptionItem)
                        .ThenInclude(i => i.Medication)
                .FirstOrDefaultAsync(v => v.Id == id);

        private async Task LoadMedicationsAsync()
        {
            var meds = await _context.Medications.OrderBy(m => m.Name).ToListAsync();
            MedicationItems = [.. meds.Select(m => new SelectListItem(m.Name, m.Id.ToString()))];
            MedicationCosts = meds.ToDictionary(m => m.Id, m => m.Cost);
        }

        public class PrescriptionInput
        {
            public int VisitId { get; set; }
            public string Description { get; set; } = "";
            public List<ItemInput> Items { get; set; } = [];
        }

        public class ItemInput
        {
            public int MedicationId { get; set; }
            public string Dosage { get; set; } = "";
            public string Amount { get; set; } = "";
            public string Price { get; set; } = "";
        }
    }
}
