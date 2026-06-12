using ClinicManager.Dtos.MedicalFiles;
using ClinicManager.Models;
using Riok.Mapperly.Abstractions;

namespace ClinicManager.Mapping
{
    [Mapper]
    public partial class MedicalFileMapper
    {
        public partial MedicalFileDto ToDto(MedicalFile file);
        public partial List<MedicalFileDto> ToDtoList(List<MedicalFile> files);
    }
}
