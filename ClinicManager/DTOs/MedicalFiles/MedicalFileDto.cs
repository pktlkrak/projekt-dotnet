namespace ClinicManager.Dtos.MedicalFiles
{
    public class MedicalFileDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Path { get; set; } = "";
    }
}
