using ClinicManager.Dtos.Patients;

namespace ClinicManager.Services
{
    // Shared patient operations available to Admin and RegistrationWorker.
    public interface IPatientService
    {
        Task<List<PatientDto>> GetActivePatientsAsync(string? search);

        Task<PatientDto?> GetPatientAsync(int id);

        Task<PatientFormDto?> GetPatientForEditAsync(int id);

        Task CreatePatientAsync(PatientFormDto dto);

        Task<bool> UpdatePatientAsync(int id, PatientFormDto dto);

        Task<bool> SoftDeletePatientAsync(int id);
    }
}