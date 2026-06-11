using System.Collections;

namespace ClinicManager.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Pesel { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsDeleted { get; set; } = false; // soft delete (RODO) 

        public string InsuranceNumber { get; set; }

        ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}
