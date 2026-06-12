using ClinicManager.Dtos.Visits;
using ClinicManager.Models;
using Riok.Mapperly.Abstractions;

namespace ClinicManager.Mapping
{
    [Mapper]
    public partial class VisitMapper
    {
        public partial VisitDto ToDto(Visit visit);
        public partial List<VisitDto> ToDtoList(List<Visit> visits);

        [MapperIgnoreTarget(nameof(VisitDetailDto.ProcedureId))]
        public partial VisitDetailDto ToDetailDto(Visit visit);

        [MapperIgnoreTarget(nameof(Visit.Id))]
        [MapperIgnoreTarget(nameof(Visit.Patient))]
        [MapperIgnoreTarget(nameof(Visit.Doctor))]
        [MapperIgnoreTarget(nameof(Visit.Procedure))]
        [MapperIgnoreTarget(nameof(Visit.Prescriptions))]
        [MapperIgnoreTarget(nameof(Visit.Survey))]
        [MapperIgnoreTarget(nameof(Visit.Diagnosis))]
        [MapperIgnoreTarget(nameof(Visit.Recommendations))]
        public partial Visit ToEntity(VisitCreateDto dto);
    }
}
