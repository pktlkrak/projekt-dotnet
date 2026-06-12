namespace ClinicManager.Dtos.Procedures
{
    public class ProcedureDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public double Cost { get; set; }
    }
}
