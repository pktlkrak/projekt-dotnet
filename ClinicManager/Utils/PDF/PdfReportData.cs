namespace ClinicManager.Utils.PDF;

public class PdfReportData(string title, string subtitle)
{
    public string Title { get; set; } = title;

    public string Subtitle { get; set; } = subtitle;

    public PdfSection[] TopLeft { get; set; } = [];

    public PdfSection[] TopRight { get; set; } = [];

    public PdfSection[] Middle { get; set; } = [];

    public PdfSection[] BottomLeft { get; set; } = [];

    public PdfSection[] BottomRight { get; set; } = [];
}
