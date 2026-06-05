namespace ClinicManager.Models;

public class Patient
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Pesel { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public bool IsDeleted { get; set; } = false; // soft delete (RODO)

    // Nawigacja
    public MedicalRecord? MedicalRecord { get; set; }
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
