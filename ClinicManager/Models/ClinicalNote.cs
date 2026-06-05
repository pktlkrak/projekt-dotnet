namespace ClinicManager.Models;

public class ClinicalNote
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public string AuthorId { get; set; } = string.Empty; // FK do ApplicationUser
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ClinicalNoteType Type { get; set; }

    // Nawigacja
    public Visit Visit { get; set; } = null!;
    public ApplicationUser Author { get; set; } = null!;
}
