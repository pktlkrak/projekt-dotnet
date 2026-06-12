using System.Globalization;
using ClinicManager.Data;
using ClinicManager.Dtos.Visits;
using ClinicManager.Mapping;
using ClinicManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class VisitService : IVisitService
    {
        private readonly AppDbContext _context;
        private readonly VisitMapper _mapper;

        public VisitService(AppDbContext context, VisitMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<VisitDto>> GetTodaysVisitsForDoctorAsync(string doctorId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var visits = await _context.Visits
                .Include(v => v.Patient)
                .Where(v => v.DoctorId == doctorId && v.ScheduledAt >= today && v.ScheduledAt < tomorrow)
                .OrderBy(v => v.ScheduledAt)
                .ToListAsync();

            return _mapper.ToDtoList(visits);
        }

        public async Task<List<VisitDto>> GetVisitsForPatientAsync(int patientId)
        {
            var visits = await _context.Visits
                .Include(v => v.Doctor)
                .Where(v => v.PatientId == patientId)
                .OrderByDescending(v => v.ScheduledAt)
                .ToListAsync();

            return _mapper.ToDtoList(visits);
        }

        public async Task CreateVisitAsync(VisitCreateDto dto)
        {
            var visit = _mapper.ToEntity(dto);
            visit.Procedure = dto.ProcedureId > 0 ? await _context.Procedures.FindAsync(dto.ProcedureId) : null;
            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();
        }

        public async Task<VisitDetailDto?> GetVisitDetailAsync(int id)
        {
            var visit = await LoadVisitAsync(id);
            return visit == null ? null : MapDetail(visit);
        }

        public async Task<VisitDetailDto?> UpdateVisitAsync(int id, VisitDetailDto dto, bool isDoctor)
        {
            var existing = await LoadVisitAsync(id);
            if (existing == null) return null;

            existing.Status = dto.Status;

            if (isDoctor)
            {
                existing.Procedure = dto.ProcedureId.HasValue ? await _context.Procedures.FindAsync(dto.ProcedureId.Value) : null;
                existing.Cost = dto.Cost;
                existing.Survey = dto.Survey;
                existing.Diagnosis = dto.Diagnosis;
                existing.Recommendations = dto.Recommendations;
            }
            else
            {
                existing.ScheduledAt = dto.ScheduledAt;
            }

            await _context.SaveChangesAsync();
            await _context.Entry(existing).Reference(v => v.Procedure).LoadAsync();
            return MapDetail(existing);
        }

        public async Task<VisitDetailDto?> FinishVisitAsync(int id, VisitDetailDto dto)
        {
            var existing = await LoadVisitAsync(id);
            if (existing == null) return null;

            existing.Status = VisitStatus.Finished;
            existing.Procedure = dto.ProcedureId.HasValue ? await _context.Procedures.FindAsync(dto.ProcedureId.Value) : null;
            existing.Cost = dto.Cost;
            existing.Survey = dto.Survey;
            existing.Diagnosis = dto.Diagnosis;
            existing.Recommendations = dto.Recommendations;

            await _context.SaveChangesAsync();
            return MapDetail(existing);
        }

        public async Task AddPrescriptionAsync(PrescriptionFormDto dto)
        {
            var validItems = dto.Items.Where(i => i.MedicationId > 0).ToList();

            var prescription = new Perscription
            {
                VisitId = dto.VisitId,
                Description = dto.Description,
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
        }

        public async Task<PrescriptionOwnerDto?> GetPrescriptionOwnerAsync(int prescriptionId)
        {
            var prescription = await _context.Perscriptions.FindAsync(prescriptionId);
            if (prescription?.VisitId == null) return null;

            var visit = await _context.Visits.FindAsync(prescription.VisitId.Value);
            if (visit == null) return null;

            return new PrescriptionOwnerDto(prescription.VisitId.Value, visit.DoctorId);
        }

        public async Task<bool> DeletePrescriptionAsync(int prescriptionId)
        {
            var prescription = await _context.Perscriptions
                .Include(p => p.PerscriptionItem)
                .FirstOrDefaultAsync(p => p.Id == prescriptionId);

            if (prescription == null) return false;

            _context.Perscriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return true;
        }

        private VisitDetailDto MapDetail(Visit visit)
        {
            var dto = _mapper.ToDetailDto(visit);
            dto.ProcedureId = visit.Procedure?.Id;
            return dto;
        }

        private Task<Visit?> LoadVisitAsync(int id) =>
            _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Include(v => v.Procedure)
                .Include(v => v.Prescriptions)
                    .ThenInclude(p => p.PerscriptionItem)
                        .ThenInclude(i => i.Medication)
                .FirstOrDefaultAsync(v => v.Id == id);
    }
}
