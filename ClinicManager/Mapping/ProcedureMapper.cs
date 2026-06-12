using ClinicManager.Dtos.Procedures;
using ClinicManager.Models;
using Riok.Mapperly.Abstractions;

namespace ClinicManager.Mapping
{
    [Mapper]
    public partial class ProcedureMapper
    {
        public partial ProcedureDto ToDto(Procedure procedure);
        public partial List<ProcedureDto> ToDtoList(List<Procedure> procedures);
        public partial ProcedureFormDto ToFormDto(Procedure procedure);

        public partial Procedure ToEntity(ProcedureFormDto dto);

        [MapperIgnoreTarget(nameof(Procedure.Id))]
        public partial void UpdateEntity(ProcedureFormDto dto, Procedure entity);
    }
}
