using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Dtos.Procedures
{
    public class ProcedureFormDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be a positive value.")]
        public double Cost { get; set; }
    }
}
