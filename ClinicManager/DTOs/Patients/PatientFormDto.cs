using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Dtos.Patients
{
    public class PatientFormDto
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL must be exactly 11 digits.")]
        public string Pesel { get; set; } = "";

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Phone number must be exactly 9 digits.")]
        public string PhoneNumber { get; set; } = "";

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = "";

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string InsuranceNumber { get; set; } = "";
    }
}