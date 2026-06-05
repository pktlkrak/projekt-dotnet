namespace ClinicManager.Models
{
    public class PerscriptionItem
    {
        public int Id { get; set; }
        public string Dosage { get; set; }

        public string Amount { get; set; }

        public double Price { get; set; }

        public Medication Medication { get; set; }
    }
}
