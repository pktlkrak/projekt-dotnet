namespace ClinicManager.Models;

public class Medication
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string? Description { get; set; }

    // Nawigacja
    public ICollection<PrescribedMedication> PrescribedMedications { get; set; } = new List<PrescribedMedication>();
}
