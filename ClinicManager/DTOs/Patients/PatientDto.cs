namespace ClinicManager.Dtos.Patients
{
    public class PatientDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Pesel { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public bool IsDeleted { get; set; }
        public string InsuranceNumber { get; set; } = "";
    }
}