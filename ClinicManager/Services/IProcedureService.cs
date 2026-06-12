using ClinicManager.Dtos.Procedures;

namespace ClinicManager.Services
{
    public interface IProcedureService
    {
        Task<List<ProcedureDto>> GetAllProceduresAsync();
        Task<ProcedureFormDto?> GetProcedureForEditAsync(int id);
        Task CreateProcedureAsync(ProcedureFormDto dto);
        Task<bool> UpdateProcedureAsync(int id, ProcedureFormDto dto);
        Task<ProcedureDto?> DeleteProcedureAsync(int id);
    }
}
