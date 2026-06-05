namespace ClinicManager.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string InsuranceNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // Nawigacja
        public Patient Patient { get; set; } = null!;
        public ICollection<MedicalDocument> Documents { get; set; } = new List<MedicalDocument>();
    }
}
