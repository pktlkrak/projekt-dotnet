using ClinicManager.Dtos.Medications;

namespace ClinicManager.Dtos.Visits
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public List<PrescriptionItemDto> PerscriptionItem { get; set; } = [];
    }

    public class PrescriptionItemDto
    {
        public int Id { get; set; }
        public int MedicationId { get; set; }
        public MedicationDto? Medication { get; set; }
        public string Dosage { get; set; } = "";
        public string Amount { get; set; } = "";
        public double Price { get; set; }
    }
}
