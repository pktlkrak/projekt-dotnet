using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClinicManager.Utils.PDF;

public static class PdfReportWriter
{
    static PdfReportWriter()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] GenerateBytes(PdfReportData data)
        => BuildDocument(data).GeneratePdf();

    public static void SaveToFile(PdfReportData data, string filePath)
        => BuildDocument(data).GeneratePdf(filePath);

    private static Document BuildDocument(PdfReportData data) =>
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(t => t.FontSize(10).FontFamily("Arial"));

                page.Header().PaddingBottom(12).Column(header =>
                {
                    header.Item()
                        .Text(data.Title)
                        .SemiBold()
                        .FontSize(18);

                    if (!string.IsNullOrWhiteSpace(data.Subtitle))
                    {
                        header.Item()
                            .PaddingTop(2)
                            .Text(data.Subtitle)
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);
                    }

                    header.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().Column(body =>
                {
                    body.Spacing(0);

                    if (data.TopLeft.Length > 0 || data.TopRight.Length > 0)
                    {
                        body.Item().PaddingVertical(10).Row(row =>
                        {
                            row.RelativeItem().Column(col => RenderSections(col, data.TopLeft));
                            row.ConstantItem(24);
                            row.RelativeItem().Column(col => RenderSections(col, data.TopRight));
                        });

                        body.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    }

                    if (data.Middle.Length > 0)
                    {
                        body.Item().PaddingVertical(10)
                            .Column(col => RenderSections(col, data.Middle));

                        body.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    }

                    if (data.BottomLeft.Length > 0 || data.BottomRight.Length > 0)
                    {
                        body.Item().PaddingVertical(10).Row(row =>
                        {
                            row.RelativeItem().Column(col => RenderSections(col, data.BottomLeft));
                            row.ConstantItem(24);
                            row.RelativeItem().Column(col => RenderSections(col, data.BottomRight));
                        });
                    }
                });
            });
        });


    private static void RenderSections(ColumnDescriptor col, PdfSection[] sections)
    {
        col.Spacing(8);
        foreach (var section in sections)
            col.Item().Column(inner =>
            {
                inner.Spacing(2);
                inner.Item().Text(section.Title).SemiBold().FontSize(10);
                inner.Item().PaddingLeft(2).Text(section.Content).FontSize(9.5f).FontColor(Colors.Grey.Darken3);
            });
    }
}
