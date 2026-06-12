using ClinicManager.Data;
using ClinicManager.Dtos.MedicalFiles;
using ClinicManager.Mapping;
using ClinicManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services
{
    public class MedicalFileService : IMedicalFileService
    {
        private readonly AppDbContext _context;
        private readonly MedicalFileMapper _mapper;

        public MedicalFileService(AppDbContext context, MedicalFileMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MedicalFileDto>> GetFilesForPatientAsync(int patientId)
        {
            var files = await _context.MedicalFiles.Where(m => m.PatientId == patientId).ToListAsync();
            return _mapper.ToDtoList(files);
        }

        public async Task AddFileAsync(int patientId, string path)
        {
            _context.MedicalFiles.Add(new MedicalFile { PatientId = patientId, Path = path });
            await _context.SaveChangesAsync();
        }

        public async Task<MedicalFileDto?> GetFileAsync(int fileId)
        {
            var file = await _context.MedicalFiles.FindAsync(fileId);
            return file == null ? null : _mapper.ToDto(file);
        }

        public async Task<bool> DeleteFileAsync(int fileId)
        {
            var file = await _context.MedicalFiles.FindAsync(fileId);
            if (file == null) return false;

            _context.MedicalFiles.Remove(file);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
