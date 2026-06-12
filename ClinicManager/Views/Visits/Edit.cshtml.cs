using System.Text;
using ClinicManager.Dtos.Visits;
using ClinicManager.Models;
using ClinicManager.Services;
using ClinicManager.Utils.PDF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicManager.Views.Visits
{
    [Authorize(Roles = "Admin,Doctor,RegistrationWorker")]
    public class EditModel : PageModel
    {
        private readonly IVisitService _visitService;
        private readonly IMedicationService _medicationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(IVisitService visitService, IMedicationService medicationService, UserManager<ApplicationUser> userManager)
        {
            _visitService = visitService;
            _medicationService = medicationService;
            _userManager = userManager;
        }

        [BindProperty]
        public VisitDetailDto Visit { get; set; } = default!;

        [BindProperty]
        public PrescriptionFormDto NewPrescription { get; set; } = new();

        [BindProperty]
        public string ReturnUrl { get; set; } = "/Doctor/Index";

        [TempData]
        public string? StatusMessage { get; set; }

        public List<SelectListItem> MedicationItems { get; set; } = [];
        public Dictionary<int, double> MedicationCosts { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var visit = await _visitService.GetVisitDetailAsync(id.Value);
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

            var visit = await _visitService.GetVisitDetailAsync(id.Value);
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

            var prescriptionSections = visit.Prescriptions.Select((rx, i) =>
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

            var visit = await _visitService.GetVisitDetailAsync(id.Value);
            if (visit == null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var userId = _userManager.GetUserId(User);
                if (visit.DoctorId != userId) return Forbid();
            }

            var rx = visit.Prescriptions
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
            var isDoctor = User.IsInRole("Doctor");

            if (isDoctor)
            {
                var current = await _visitService.GetVisitDetailAsync(Visit.Id);
                if (current == null) return NotFound();

                var userId = _userManager.GetUserId(User);
                if (current.DoctorId != userId) return Forbid();
            }

            var updated = await _visitService.UpdateVisitAsync(Visit.Id, Visit, isDoctor);
            if (updated == null) return NotFound();

            StatusMessage = "Appointment saved.";
            Visit = updated;
            await LoadMedicationsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostFinishAsync()
        {
            if (User.IsInRole("Doctor"))
            {
                var current = await _visitService.GetVisitDetailAsync(Visit.Id);
                if (current == null) return NotFound();

                var userId = _userManager.GetUserId(User);
                if (current.DoctorId != userId) return Forbid();
            }

            var updated = await _visitService.FinishVisitAsync(Visit.Id, Visit);
            if (updated == null) return NotFound();

            var url = ReturnUrl.StartsWith('/') ? ReturnUrl : "/Doctor/Index";
            return LocalRedirect(url);
        }

        public async Task<IActionResult> OnPostAddPrescriptionAsync()
        {
            var doctorId = _userManager.GetUserId(User);
            var visit = await _visitService.GetVisitDetailAsync(NewPrescription.VisitId);

            if (visit == null) return NotFound();
            if (visit.DoctorId != doctorId) return Forbid();

            var validItemCount = NewPrescription.Items.Count(i => i.MedicationId > 0);
            if (validItemCount == 0)
            {
                ModelState.AddModelError("", "At least one medication is required.");
                Visit = visit;
                await LoadMedicationsAsync();
                return Page();
            }

            await _visitService.AddPrescriptionAsync(NewPrescription);

            StatusMessage = "Prescription added.";
            return RedirectToPage(new { id = NewPrescription.VisitId });
        }

        public async Task<IActionResult> OnPostDeletePrescriptionAsync(int prescriptionId)
        {
            var owner = await _visitService.GetPrescriptionOwnerAsync(prescriptionId);
            if (owner == null) return NotFound();

            if (User.IsInRole("Doctor"))
            {
                var doctorId = _userManager.GetUserId(User);
                if (owner.DoctorId != doctorId) return Forbid();
            }

            await _visitService.DeletePrescriptionAsync(prescriptionId);

            StatusMessage = "Prescription deleted.";
            return RedirectToPage(new { id = owner.VisitId });
        }

        private async Task LoadMedicationsAsync()
        {
            var meds = await _medicationService.GetAllMedicationsAsync();
            MedicationItems = [.. meds.Select(m => new SelectListItem(m.Name, m.Id.ToString()))];
            MedicationCosts = meds.ToDictionary(m => m.Id, m => m.Cost);
        }
    }
}
