using ClinicManager.Utils.PDF;

namespace ClinicManagerTests;

[TestFixture]
public class PdfReportWriterTests
{
    [Test]
    public void GenerateBytes_MinimalData_ReturnsPdf()
    {
        var data = new PdfReportData("Visit Report", "Dr. Smith · 01 Jan 2026");

        var bytes = PdfReportWriter.GenerateBytes(data);

        Assert.That(bytes, Is.Not.Empty);
        Assert.That(bytes[..4], Is.EqualTo("%PDF"u8.ToArray()));
    }

    [Test]
    public void GenerateBytes_WithTopSections_ReturnsPdf()
    {
        var data = new PdfReportData("Visit Report", "subtitle")
        {
            TopLeft = [new("Patient", "Kowalski Jan\nPESEL: 12345678901")],
            TopRight = [new("Visit", "Date: 01 Jan 2026\nDoctor: Smith John")],
        };

        var bytes = PdfReportWriter.GenerateBytes(data);

        Assert.That(bytes, Is.Not.Empty);
        Assert.That(bytes[..4], Is.EqualTo("%PDF"u8.ToArray()));
    }

    [Test]
    public void GenerateBytes_WithAllSections_ReturnsPdf()
    {
        var data = new PdfReportData("Visit Report", "subtitle")
        {
            TopLeft  = [new("Patient", "Kowalski Jan")],
            TopRight = [new("Visit", "01 Jan 2026")],
            Middle   = [new("Interview", "No complaints."), new("Diagnosis", "Healthy.")],
            BottomLeft  = [new("Prescription #1", "• Ibuprofen | 3x daily | 20 tabs | 9,99 zł\nTotal: 9,99 zł")],
            BottomRight = [new("Grand Total", "9,99 zł")],
        };

        var bytes = PdfReportWriter.GenerateBytes(data);

        Assert.That(bytes, Is.Not.Empty);
        Assert.That(bytes[..4], Is.EqualTo("%PDF"u8.ToArray()));
    }

    [Test]
    public void GenerateBytes_EmptySections_DoesNotThrow()
    {
        var data = new PdfReportData("Title Only", "");

        Assert.DoesNotThrow(() => PdfReportWriter.GenerateBytes(data));
    }
}
