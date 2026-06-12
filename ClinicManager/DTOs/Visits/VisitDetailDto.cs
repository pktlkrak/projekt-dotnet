using ClinicManager.Dtos.Common;
using ClinicManager.Dtos.Patients;
using ClinicManager.Models;

namespace ClinicManager.Dtos.Visits
{
    public class VisitDetailDto
    {
        public int Id { get; set; }
        public string DoctorId { get; set; } = "";
        public DateTime ScheduledAt { get; set; }
        public VisitStatus Status { get; set; }

        public string Survey { get; set; } = "";
        public string Diagnosis { get; set; } = "";
        public string Recommendations { get; set; } = "";

        public PatientDto? Patient { get; set; }
        public PersonNameDto? Doctor { get; set; }

        public List<PrescriptionDto> Prescriptions { get; set; } = [];
    }
}
