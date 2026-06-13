using ClinicManager.Dtos.Medications;
using ClinicManager.Models;
using Riok.Mapperly.Abstractions;

namespace ClinicManager.Mapping
{
    [Mapper]
    public partial class MedicationMapper
    {
        public partial MedicationDto ToDto(Medication medication);
        public partial List<MedicationDto> ToDtoList(List<Medication> medications);
        public partial MedicationFormDto ToFormDto(Medication medication);

        [MapperIgnoreTarget(nameof(Medication.Id))]
        public partial Medication ToEntity(MedicationFormDto dto);

        [MapperIgnoreTarget(nameof(Medication.Id))]
        public partial void UpdateEntity(MedicationFormDto dto, Medication entity);
    }
}
