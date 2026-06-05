namespace ClinicManager.Models
{
    public class MedicalDocument
    {
        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Nawigacja
        public MedicalRecord MedicalRecord { get; set; } = null!;
    }
}
