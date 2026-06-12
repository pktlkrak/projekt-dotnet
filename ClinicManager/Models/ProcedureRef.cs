namespace ClinicManager.Models
{
    public class ProcedureRef
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public int ProcedureId { get; set; }
        public double Cost { get; set; }

        public Visit Visit { get; set; } = null!;
        public Procedure Procedure { get; set; } = null!;
    }
}
