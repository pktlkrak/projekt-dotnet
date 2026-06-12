using ClinicManager.Dtos.Common;
using ClinicManager.Models;

namespace ClinicManager.Dtos.Visits
{
    public class VisitDto
    {
        public int Id { get; set; }
        public DateTime ScheduledAt { get; set; }
        public VisitStatus Status { get; set; }

        public PersonNameDto? Patient { get; set; }
        public PersonNameDto? Doctor { get; set; }
    }
}
