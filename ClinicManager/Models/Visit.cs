namespace ClinicManager.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string DoctorId { get; set; } = string.Empty; // FK do ApplicationUser
        public DateTime ScheduledAt { get; set; }
        public VisitStatus Status { get; set; } = VisitStatus.Scheduled;

        // Nawigacja
        public Patient Patient { get; set; } = null!;
        public ApplicationUser Doctor { get; set; } = null!;
        public ICollection<ProcedurePerformed> ProceduresPerformed { get; set; } = new List<ProcedurePerformed>();
        public ICollection<ClinicalNote> ClinicalNotes { get; set; } = new List<ClinicalNote>();
    }
}
