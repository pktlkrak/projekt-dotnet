using ClinicManager.Dtos.Patients;
using ClinicManager.Models;
using Riok.Mapperly.Abstractions;

namespace ClinicManager.Mapping
{
    [Mapper]
    public partial class PatientMapper
    {
        public partial PatientDto ToDto(Patient patient);

        public partial List<PatientDto> ToDtoList(List<Patient> patients);

        public partial PatientFormDto ToFormDto(Patient patient);

        [MapperIgnoreTarget(nameof(Patient.Id))]
        [MapperIgnoreTarget(nameof(Patient.IsDeleted))]
        [MapperIgnoreTarget(nameof(Patient.Visits))]
        public partial Patient ToEntity(PatientFormDto dto);

        [MapperIgnoreTarget(nameof(Patient.Id))]
        [MapperIgnoreTarget(nameof(Patient.IsDeleted))]
        [MapperIgnoreTarget(nameof(Patient.Visits))]
        public partial void UpdateEntity(PatientFormDto dto, Patient entity);
    }
}
