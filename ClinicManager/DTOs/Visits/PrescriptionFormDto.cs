namespace ClinicManager.Dtos.Visits
{
    public class PrescriptionFormDto
    {
        public int VisitId { get; set; }
        public string Description { get; set; } = "";
        public List<PrescriptionItemFormDto> Items { get; set; } = [];
    }

    public class PrescriptionItemFormDto
    {
        public int MedicationId { get; set; }
        public string Dosage { get; set; } = "";
        public string Amount { get; set; } = "";
        public string Price { get; set; } = "";
    }
}
