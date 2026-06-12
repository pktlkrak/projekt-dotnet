using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Dtos.Medications
{
    public class MedicationFormDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be a positive value.")]
        public double Cost { get; set; }
    }
}
