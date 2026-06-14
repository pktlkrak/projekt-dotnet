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

        var today = DateTime.Today;
        var report = await reportService.GetDailyReportAsync(today.Year, today.Month, today.Day);

        if (report is null)
        {
            logger.LogWarning("Daily report returned null for {Date:yyyy-MM-dd}.", today);
            return;
        }

        logger.LogInformation(
            "Generated daily report for {Date:yyyy-MM-dd} with {Count} visit(s).",
            today, report.Visits.Count);

        var pdf = PdfAdminReportWriter.GenerateDailyReport(report);
        // await File.WriteAllBytesAsync($"reports/daily-{today:yyyy-MM-dd}.pdf", pdf, ct);
        await emailService.SendAsync(
            emailService.AdminAddress,
            $"Daily report for {today}",
            "Attached you may find the daily report for today.",
            false,
            new EmailAttachment(pdf, $"report-{today}.pdf", "application/pdf")
        );
        logger.LogInformation("Report sent.");
    }
}
