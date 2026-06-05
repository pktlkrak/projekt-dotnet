namespace ClinicManager.Models
{
    public class Perscription
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public ICollection<PerscriptionItem> PerscriptionItem { get; set; }
    }
}
