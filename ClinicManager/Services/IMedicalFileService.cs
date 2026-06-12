using ClinicManager.Dtos.MedicalFiles;

namespace ClinicManager.Services
{
    public interface IMedicalFileService
    {
        Task<List<MedicalFileDto>> GetFilesForPatientAsync(int patientId);
        Task AddFileAsync(int patientId, string path);
        Task<MedicalFileDto?> GetFileAsync(int fileId);
        Task<bool> DeleteFileAsync(int fileId);
    }
}
