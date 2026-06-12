using ClinicManager.Dtos.Visits;

namespace ClinicManager.Services
{
    public interface IVisitService
    {
        Task<List<VisitDto>> GetTodaysVisitsForDoctorAsync(string doctorId);
        Task<List<VisitDto>> GetVisitsForPatientAsync(int patientId);

        Task CreateVisitAsync(VisitCreateDto dto);

        Task<VisitDetailDto?> GetVisitDetailAsync(int id);
        Task<VisitDetailDto?> UpdateVisitAsync(int id, VisitDetailDto dto, bool isDoctor);
        Task<VisitDetailDto?> FinishVisitAsync(int id, VisitDetailDto dto);

        Task AddPrescriptionAsync(PrescriptionFormDto dto);
        Task<PrescriptionOwnerDto?> GetPrescriptionOwnerAsync(int prescriptionId);
        Task<bool> DeletePrescriptionAsync(int prescriptionId);
    }

    public record PrescriptionOwnerDto(int VisitId, string DoctorId);
}
