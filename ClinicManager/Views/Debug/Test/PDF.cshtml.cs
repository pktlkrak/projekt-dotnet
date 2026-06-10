using ClinicManager.Utils.PDF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManager.Views.Debug.Test;

public class PdfTestModel : PageModel
{
    public void OnGet() { }

    public IActionResult OnGetDownload()
    {
        var data = new PdfReportData("Test Title", "Test Subtitle")
        {
            TopLeft =
            [
                new("Top-Left Section 1", "Content for top-left section 1."),
                new("Top-Left Section 2", "Content for top-left section 2."),
            ],
            TopRight =
            [
                new("Top-Right Section 1", "Content for top-right section 1."),
                new("Top-Right Section 2", "Content for top-right section 2."),
            ],
            Middle =
            [
                new("Middle Section 1", "Content for middle section 1"),
                new("Middle Section 2", "Content for middle section 2."),
            ],
            BottomLeft =
            [
                new("Bottom-Left Section 1", "Content for bottom-left section 1."),
            ],
            BottomRight =
            [
                new("Bottom-Right Section 1", "Content for bottom-right section 1."),
            ],
        };

        var bytes = PdfReportWriter.GenerateBytes(data);
        return File(bytes, "application/pdf", "test.pdf");
    }
}
