namespace ClinicManager.Utils.PDF;

public class PdfSection(string title, string content)
{
    public string Title { get; set; } = title;
    public string Content { get; set; } = content;
}
