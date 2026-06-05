namespace ClinicManager.Models
{
    public class Visit
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public VisitStastus Status { get; set; }

        public ICollection<Procedure> Procedures { get; set; }
        public ICollection<Perscription> Reciepts { get; set; }

        // Notatki Kliniczne

        public string Survey { get; set; } // wywiad
        public string Diagnosis { get; set; } // rozpoznanie
        public string Recommendations { get; set; } // zalecenia

    }

    public enum VisitStastus
    {
        Scheduled,
        InProgress,
        Finished,
        Cancelled
    }
}
