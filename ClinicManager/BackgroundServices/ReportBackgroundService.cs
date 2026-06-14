using ClinicManager.Utils.Email;
using ClinicManager.Utils.PDF;
using ClinicManager.Models;
using ClinicManager.Services;


public class ReportBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<ReportBackgroundService> logger
) : BackgroundService
{
    // TODO: Change this to 1 day for actual deploymentL
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_interval);

        do
        {
            try
            {
                await RunAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break; // shutdown — exit quietly
            }
            catch (Exception ex)
            {
                // Swallow so one bad run doesn't kill the loop.
                logger.LogError(ex, "Report background run failed.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RunAsync(CancellationToken ct)
    {
        // IReportService and IEmailService are scoped, so resolve them
        // inside a fresh scope on each tick rather than injecting them directly.
        using var scope = scopeFactory.CreateScope();
        var reportService = scope.ServiceProvider.GetRequiredService<IReportService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var tomorrow = DateTime.Today.AddDays(1);
        var report = await reportService.GetDailyReportAsync(tomorrow.Year, tomorrow.Month, tomorrow.Day);

        if (report is null)
        {
            logger.LogWarning("Daily report returned null for {Date:yyyy-MM-dd}.", tomorrow);
            return;
        }

        logger.LogInformation(
            "Generated daily report for {Date:yyyy-MM-dd} with {Count} visit(s).",
            tomorrow, report.Visits.Count);

        var pdf = PdfAdminReportWriter.GenerateDailyReport(report);
        // await File.WriteAllBytesAsync($"reports/daily-{tomorrow:yyyy-MM-dd}.pdf", pdf, ct);
        await emailService.SendAsync(
            emailService.AdminAddress,
            $"Daily report for {tomorrow}",
            "Attached you may find the daily report for tomorrow.",
            false,
            new EmailAttachment(pdf, $"report-{tomorrow:yyyy-MM-dd}.pdf", "application/pdf")
        );
        logger.LogInformation("Report sent.");
    }
}
