using ClinicManager.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace ClinicManager.Utils.PDF;

public static class PdfAdminReportWriter
{
    static PdfAdminReportWriter()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] GenerateMonthlyReport(MonthlyReportData data)
    {
        var monthName = new DateTime(data.Year, data.Month, 1).ToString("MMMM yyyy", CultureInfo.InvariantCulture);

        return Document.Create(container => container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.DefaultTextStyle(t => t.FontSize(10).FontFamily("Arial"));

            page.Header().PaddingBottom(12).Column(col =>
            {
                col.Item().Text($"Monthly Procedure Report — {monthName}").SemiBold().FontSize(16);
                col.Item().PaddingTop(2).Text($"Dr. {data.DoctorName}").FontSize(10).FontColor(Colors.Grey.Darken1);
                col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });

            page.Content().PaddingTop(10).Column(body =>
            {
                body.Spacing(16);

                // Visits table
                body.Item().Text("Visits").SemiBold().FontSize(11);
                body.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(90);   // date
                        cols.RelativeColumn(2);    // patient
                        cols.RelativeColumn();     // status
                        cols.RelativeColumn(3);    // procedures
                        cols.ConstantColumn(65);   // total
                    });

                    table.Header(header =>
                    {
                        foreach (var label in new[] { "Date", "Patient", "Status", "Procedures", "Total" })
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text(label).SemiBold().FontSize(9);
                    });

                    foreach (var (visit, idx) in data.Visits.Select((v, i) => (v, i)))
                    {
                        var bg = idx % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                        var procs = visit.Procedures.Count > 0
                            ? string.Join("\n", visit.Procedures.Select(p => $"• {p.Name}  {p.Cost:C}"))
                            : "—";
                        var total = visit.Procedures.Sum(p => p.Cost);

                        table.Cell().Background(bg).Padding(5).Text(visit.ScheduledAt.ToString("dd.MM.yyyy HH:mm")).FontSize(9);
                        table.Cell().Background(bg).Padding(5).Text(visit.PatientName).FontSize(9);
                        table.Cell().Background(bg).Padding(5).Text(visit.Status.ToString()).FontSize(9);
                        table.Cell().Background(bg).Padding(5).Text(procs).FontSize(9);
                        table.Cell().Background(bg).Padding(5).AlignRight().Text(total.ToString("C")).FontSize(9);
                    }

                    if (data.Visits.Count == 0)
                    {
                        table.Cell().ColumnSpan(5).Padding(10).AlignCenter()
                            .Text("No visits recorded for this period.").FontColor(Colors.Grey.Medium);
                    }
                });

                // Summary by procedure
                var summary = data.Visits
                    .SelectMany(v => v.Procedures)
                    .GroupBy(p => p.Name)
                    .Select(g => (Name: g.Key, Count: g.Count(), Total: g.Sum(p => p.Cost)))
                    .OrderByDescending(s => s.Total)
                    .ToList();

                if (summary.Count > 0)
                {
                    body.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    body.Item().Text("Summary by Procedure").SemiBold().FontSize(11);
                    body.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(4);
                            cols.ConstantColumn(60);
                            cols.ConstantColumn(90);
                        });

                        table.Header(header =>
                        {
                            foreach (var label in new[] { "Procedure", "Count", "Total Cost" })
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text(label).SemiBold().FontSize(9);
                        });

                        foreach (var (row, idx) in summary.Select((r, i) => (r, i)))
                        {
                            var bg = idx % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                            table.Cell().Background(bg).Padding(5).Text(row.Name).FontSize(9);
                            table.Cell().Background(bg).Padding(5).AlignCenter().Text(row.Count.ToString()).FontSize(9);
                            table.Cell().Background(bg).Padding(5).AlignRight().Text(row.Total.ToString("C")).FontSize(9);
                        }

                        var grandTotal = summary.Sum(s => s.Total);
                        table.Cell().Padding(5).Text("Grand Total").SemiBold().FontSize(9);
                        table.Cell().Padding(5).AlignCenter().Text(summary.Sum(s => s.Count).ToString()).SemiBold().FontSize(9);
                        table.Cell().Padding(5).AlignRight().Text(grandTotal.ToString("C")).SemiBold().FontSize(9);
                    });
                }
            });
        })).GeneratePdf();
    }

    public static byte[] GeneratePatientReport(PatientReportData data)
    {
        return Document.Create(container => container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.DefaultTextStyle(t => t.FontSize(10).FontFamily("Arial"));

            page.Header().PaddingBottom(12).Column(col =>
            {
                col.Item().Text($"Patient Report — {data.PatientName}").SemiBold().FontSize(16);
                col.Item().PaddingTop(2)
                    .Text($"PESEL: {data.Pesel}  ·  Born: {data.DateOfBirth:dd.MM.yyyy}  ·  {data.Phone}  ·  {data.Email}")
                    .FontSize(9).FontColor(Colors.Grey.Darken1);
                col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });

            page.Content().PaddingTop(10).Column(body =>
            {
                body.Spacing(16);

                body.Item().Text("Visit History").SemiBold().FontSize(11);
                body.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(90);   // date
                        cols.RelativeColumn(2);    // doctor
                        cols.RelativeColumn();     // status
                        cols.RelativeColumn(3);    // procedures
                        cols.ConstantColumn(65);   // total
                    });

                    table.Header(header =>
                    {
                        foreach (var label in new[] { "Date", "Doctor", "Status", "Procedures", "Total" })
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text(label).SemiBold().FontSize(9);
                    });

                    foreach (var (visit, idx) in data.Visits.Select((v, i) => (v, i)))
                    {
                        var bg = idx % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                        var procs = visit.Procedures.Count > 0
                            ? string.Join("\n", visit.Procedures.Select(p => $"• {p.Name}  {p.Cost:C}"))
                            : "—";
                        var total = visit.Procedures.Sum(p => p.Cost);

                        table.Cell().Background(bg).Padding(5).Text(visit.ScheduledAt.ToString("dd.MM.yyyy HH:mm")).FontSize(9);
                        table.Cell().Background(bg).Padding(5).Text(visit.DoctorName).FontSize(9);
                        table.Cell().Background(bg).Padding(5).Text(visit.Status.ToString()).FontSize(9);
                        table.Cell().Background(bg).Padding(5).Text(procs).FontSize(9);
                        table.Cell().Background(bg).Padding(5).AlignRight().Text(total.ToString("C")).FontSize(9);
                    }

                    if (data.Visits.Count == 0)
                    {
                        table.Cell().ColumnSpan(5).Padding(10).AlignCenter()
                            .Text("No visits recorded.").FontColor(Colors.Grey.Medium);
                    }
                });

                if (data.Visits.Count > 0)
                {
                    var grandTotal = data.Visits.Sum(v => v.Procedures.Sum(p => p.Cost));
                    body.Item().AlignRight()
                        .Text($"Grand Total:  {grandTotal.ToString("C")}").SemiBold().FontSize(11);
                }
            });
        })).GeneratePdf();
    }
}
