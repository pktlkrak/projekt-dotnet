namespace ClinicManager.Models;

public class ProcedurePerformed
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public int ProcedureId { get; set; }

    // Nawigacja
    public Visit Visit { get; set; } = null!;
    public Procedure Procedure { get; set; } = null!;
    public ICollection<PrescribedMedication> PrescribedMedications { get; set; } = new List<PrescribedMedication>();
}
