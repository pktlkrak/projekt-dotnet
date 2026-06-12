using ClinicManager.Data;
using ClinicManager.Dtos.Patients;
using ClinicManager.Mapping;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class PatientService : IPatientAdminService
    {
        private readonly AppDbContext _context;
        private readonly PatientMapper _mapper;
        private readonly ILogger<PatientService> _logger;

        public PatientService(AppDbContext context, PatientMapper mapper, ILogger<PatientService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<List<PatientDto>> GetActivePatientsAsync(string? search) =>
            GetPatientsAsync(includeDeleted: false, search);

        public Task<List<PatientDto>> GetAllPatientsAsync(bool includeDeleted, string? search) =>
            GetPatientsAsync(includeDeleted, search);

        private async Task<List<PatientDto>> GetPatientsAsync(bool includeDeleted, string? search)
        {
            var query = _context.Patients.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(p => !p.IsDeleted);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.FirstName.Contains(search) ||
                    p.LastName.Contains(search) ||
                    p.Pesel.Contains(search));
            }

            var patients = await query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();

            return _mapper.ToDtoList(patients);
        }

        public async Task<PatientDto?> GetPatientAsync(int id)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
            return patient == null ? null : _mapper.ToDto(patient);
        }

        public async Task<PatientFormDto?> GetPatientForEditAsync(int id)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
            return patient == null ? null : _mapper.ToFormDto(patient);
        }

        public async Task CreatePatientAsync(PatientFormDto dto)
        {
            var patient = _mapper.ToEntity(dto);
            patient.IsDeleted = false;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Patient {PatientId} ({LastName}) created", patient.Id, patient.LastName);
        }

        public async Task<bool> UpdatePatientAsync(int id, PatientFormDto dto)
        {
            var existing = await _context.Patients.FindAsync(id);
            if (existing == null) return false;

            _mapper.UpdateEntity(dto, existing);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Patients.AnyAsync(p => p.Id == id))
                    return false;
                throw;
            }

            _logger.LogInformation("Patient {PatientId} ({LastName}) updated", existing.Id, existing.LastName);
            return true;
        }

        public async Task<bool> SoftDeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return false;

            patient.IsDeleted = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Patient {PatientId} ({LastName}) soft-deleted", patient.Id, patient.LastName);
            return true;
        }

        public async Task<HardDeleteResult> HardDeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return new HardDeleteResult(false);

            var visits = await _context.Visits.Where(v => v.PatientId == id).ToListAsync();
            var visitIds = visits.Select(v => (int?)v.Id).ToList();

            var procedures = await _context.Procedures
                .Where(p => visitIds.Contains(EF.Property<int?>(p, "VisitId")))
                .ToListAsync();

            var perscriptions = await _context.Perscriptions
                .Where(p => visitIds.Contains(EF.Property<int?>(p, "VisitId")))
                .ToListAsync();

            var perscriptionIds = perscriptions.Select(p => (int?)p.Id).ToList();
            var perscriptionItems = await _context.PerscriptionItems
                .Where(pi => perscriptionIds.Contains(EF.Property<int?>(pi, "PerscriptionId")))
                .ToListAsync();

            _context.PerscriptionItems.RemoveRange(perscriptionItems);
            _context.Perscriptions.RemoveRange(perscriptions);
            _context.Procedures.RemoveRange(procedures);
            _context.Visits.RemoveRange(visits);
            _context.Patients.Remove(patient);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Patient {PatientId} ({LastName}) and all related records permanently deleted", patient.Id, patient.LastName);
                return new HardDeleteResult(true);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "Cannot permanently delete patient {PatientId}", id);
                return new HardDeleteResult(false, "Cannot permanently delete this patient because of related records that could not be removed.");
            }
        }
    }
}