using ClinicManager.Data;
using ClinicManager.Dtos.Procedures;
using ClinicManager.Mapping;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class ProcedureService : IProcedureService
    {
        private readonly AppDbContext _context;
        private readonly ProcedureMapper _mapper;

        public ProcedureService(AppDbContext context, ProcedureMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProcedureDto>> GetAllProceduresAsync()
        {
            var procedures = await _context.Procedures.OrderBy(p => p.Name).ToListAsync();
            return _mapper.ToDtoList(procedures);
        }

        public async Task<ProcedureFormDto?> GetProcedureForEditAsync(int id)
        {
            var procedure = await _context.Procedures.FindAsync(id);
            return procedure == null ? null : _mapper.ToFormDto(procedure);
        }

        public async Task CreateProcedureAsync(ProcedureFormDto dto)
        {
            var procedure = _mapper.ToEntity(dto);
            _context.Procedures.Add(procedure);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateProcedureAsync(int id, ProcedureFormDto dto)
        {
            var existing = await _context.Procedures.FindAsync(id);
            if (existing == null) return false;

            _mapper.UpdateEntity(dto, existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProcedureDto?> DeleteProcedureAsync(int id)
        {
            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null) return null;

            _context.Procedures.Remove(procedure);
            await _context.SaveChangesAsync();
            return _mapper.ToDto(procedure);
        }
    }
}
