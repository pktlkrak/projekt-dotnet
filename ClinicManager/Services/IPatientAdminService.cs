using ClinicManager.Dtos.Patients;

namespace ClinicManager.Services
{
    // Admin-only patient operations (viewing soft-deleted patients, permanent deletion),
    // in addition to the shared operations available to RegistrationWorker.
    public interface IPatientAdminService : IPatientService
    {
        Task<List<PatientDto>> GetAllPatientsAsync(bool includeDeleted, string? search);

        Task<HardDeleteResult> HardDeletePatientAsync(int id);
    }

    public record HardDeleteResult(bool Success, string? ErrorMessage = null);
}