namespace ClinicManager.Models
{
    public class MedicalFile
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        // Path relative to wwwroot, e.g. "medicalDocuments/12345678901/badanie.pdf"
        public string Path { get; set; } = "";
    }
}
