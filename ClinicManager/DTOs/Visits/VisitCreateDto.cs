using System.ComponentModel.DataAnnotations;
using ClinicManager.Models;

namespace ClinicManager.Dtos.Visits
{
    public class VisitCreateDto
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please select a doctor.")]
        public string DoctorId { get; set; } = "";

        public List<ProcedureRefDto> Procedures { get; set; } = [];

        public DateTime ScheduledAt { get; set; }
        public VisitStatus Status { get; set; }
    }
}
