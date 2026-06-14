using ClinicManager.Models;
using ClinicManager.Services;
using ClinicManager.Utils.PDF;

namespace ClinicManagerTests;

[TestFixture]
public class PdfAdminReportWriterTests
{
    // --- Monthly report ---

    [Test]
    public void GenerateMonthlyReport_NoVisits_ReturnsPdf()
    {
        var data = new MonthlyReportData("Kowalski Jan", 2026, 3, []);

        var bytes = PdfAdminReportWriter.GenerateMonthlyReport(data);

        Assert.That(bytes, Is.Not.Empty);
        Assert.That(bytes[..4], Is.EqualTo("%PDF"u8.ToArray()));
    }

    [Test]
    public void GenerateMonthlyReport_WithVisits_ReturnsPdf()
    {
        var visits = new List<MonthlyVisitRow>
        {
            new(new DateTime(2026, 3, 5, 10, 0, 0), "Nowak Anna", VisitStatus.Finished,
                [("Consultation", 150.0), ("Blood test", 80.0)]),
            new(new DateTime(2026, 3, 12, 14, 30, 0), "Wiśniewska Maria", VisitStatus.Finished,
                [("Consultation", 150.0)]),
        };
        var data = new MonthlyReportData("Kowalski Jan", 2026, 3, visits);

        var bytes = PdfAdminReportWriter.GenerateMonthlyReport(data);

        Assert.That(bytes, Is.Not.Empty);
        Assert.That(bytes[..4], Is.EqualTo("%PDF"u8.ToArray()));
    }

    [Test]
    public void GenerateMonthlyReport_SameProcedureMultipleTimes_ProducesSummary()
    {
        var visits = new List<MonthlyVisitRow>
        {
            new(new DateTime(2026, 3, 1, 9, 0, 0), "Patient A", VisitStatus.Finished, [("Consultation", 150.0)]),
            new(new DateTime(2026, 3, 8, 9, 0, 0), "Patient B", VisitStatus.Finished, [("Consultation", 150.0)]),
            new(new DateTime(2026, 3, 15, 9, 0, 0), "Patient C", VisitStatus.Finished, [("Consultation", 150.0)]),
        };
        var data = new MonthlyReportData("Smith John", 2026, 3, visits);

        Assert.DoesNotThrow(() => PdfAdminReportWriter.GenerateMonthlyReport(data));
    }

    // --- Patient report ---

    [Test]
    public void GeneratePatientReport_NoVisits_ReturnsPdf()
    {
        var data = new PatientReportData(
            "Kowalski Jan", "12345678901", new DateOnly(1985, 6, 15),
            "123456789", "jan.kowalski@example.com", "INS-001", []);

        var bytes = PdfAdminReportWriter.GeneratePatientReport(data);

        Assert.That(bytes, Is.Not.Empty);
        Assert.That(bytes[..4], Is.EqualTo("%PDF"u8.ToArray()));
    }

    [Test]
    public void GeneratePatientReport_WithVisits_ReturnsPdf()
    {
        var visits = new List<PatientVisitRow>
        {
            new(new DateTime(2025, 11, 10, 10, 0, 0), "Smith John", VisitStatus.Finished,
                [("Consultation", 150.0), ("X-Ray", 200.0)]),
            new(new DateTime(2026, 1, 20, 12, 0, 0), "Smith John", VisitStatus.Finished,
                [("Consultation", 150.0)]),
        };
        var data = new PatientReportData(
            "Nowak Anna", "98765432100", new DateOnly(1990, 3, 22),
            "987654321", "anna.nowak@example.com", "INS-999", visits);

        var bytes = PdfAdminReportWriter.GeneratePatientReport(data);

        Assert.That(bytes, Is.Not.Empty);
        Assert.That(bytes[..4], Is.EqualTo("%PDF"u8.ToArray()));
    }

    [Test]
    public void GeneratePatientReport_VisitWithNoProcedures_ReturnsPdf()
    {
        var visits = new List<PatientVisitRow>
        {
            new(new DateTime(2026, 2, 1, 8, 0, 0), "Dr. House", VisitStatus.Scheduled, []),
        };
        var data = new PatientReportData(
            "Blank Patient", "00000000000", new DateOnly(2000, 1, 1),
            "000000000", "test@test.com", "INS-000", visits);

        Assert.DoesNotThrow(() => PdfAdminReportWriter.GeneratePatientReport(data));
    }
}
