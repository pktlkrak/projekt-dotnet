using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Models
{
    public class Procedure
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be a positive value.")]
        public double Cost { get; set; }
    }
}
