namespace ClinicManager.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string DoctorId { get; set; } = "";

        public DateTime ScheduledAt { get; set; }
        public VisitStatus Status { get; set; }

        public Patient? Patient { get; set; }
        public ApplicationUser? Doctor { get; set; }

        public ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();
        public ICollection<Perscription> Reciepts { get; set; } = new List<Perscription>();

        public string Survey { get; set; } = "";
        public string Diagnosis { get; set; } = "";
        public string Recommendations { get; set; } = "";
    }

    public enum VisitStatus
    {
        Scheduled,
        InProgress,
        Finished,
        Cancelled
    }
}
