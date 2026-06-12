using ClinicManager.Dtos.Medications;

namespace ClinicManager.Services
{
    public interface IMedicationService
    {
        Task<List<MedicationDto>> GetAllMedicationsAsync();
        Task<MedicationFormDto?> GetMedicationForEditAsync(int id);
        Task CreateMedicationAsync(MedicationFormDto dto);
        Task<bool> UpdateMedicationAsync(int id, MedicationFormDto dto);
        Task<MedicationDto?> DeleteMedicationAsync(int id);
    }
}
