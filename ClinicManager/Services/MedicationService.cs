using ClinicManager.Data;
using ClinicManager.Dtos.Medications;
using ClinicManager.Mapping;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly AppDbContext _context;
        private readonly MedicationMapper _mapper;

        public MedicationService(AppDbContext context, MedicationMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MedicationDto>> GetAllMedicationsAsync()
        {
            var medications = await _context.Medications.OrderBy(m => m.Name).ToListAsync();
            return _mapper.ToDtoList(medications);
        }

        public async Task<MedicationFormDto?> GetMedicationForEditAsync(int id)
        {
            var medication = await _context.Medications.FindAsync(id);
            return medication == null ? null : _mapper.ToFormDto(medication);
        }

        public async Task CreateMedicationAsync(MedicationFormDto dto)
        {
            var medication = _mapper.ToEntity(dto);
            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateMedicationAsync(int id, MedicationFormDto dto)
        {
            var existing = await _context.Medications.FindAsync(id);
            if (existing == null) return false;

            _mapper.UpdateEntity(dto, existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MedicationDto?> DeleteMedicationAsync(int id)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication == null) return null;

            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();
            return _mapper.ToDto(medication);
        }
    }
}
