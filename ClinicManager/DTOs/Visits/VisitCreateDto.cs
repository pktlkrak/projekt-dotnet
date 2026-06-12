using System.ComponentModel.DataAnnotations;
using ClinicManager.Models;

namespace ClinicManager.Dtos.Visits
{
    public class VisitCreateDto
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please select a doctor.")]
        public string DoctorId { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Please select a procedure.")]
        public int ProcedureId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be non-negative.")]
        public double Cost { get; set; }

        public DateTime ScheduledAt { get; set; }
        public VisitStatus Status { get; set; }
    }
}
