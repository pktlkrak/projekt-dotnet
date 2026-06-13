using ClinicManager.Models;

namespace ClinicManager.Services;

public interface IReportService
{
    Task<List<ApplicationUser>> GetDoctorsAsync();
    Task<MonthlyReportData?> GetMonthlyReportAsync(int year, int month, string doctorId);
    Task<DailyReportData?> GetDailyReportAsync(int year, int month, int day);
    Task<PatientReportData?> GetPatientReportAsync(int patientId);
}

public record MonthlyReportData(
    string DoctorName,
    int Year,
    int Month,
    List<MonthlyVisitRow> Visits
);

public record MonthlyVisitRow(
    DateTime ScheduledAt,
    string PatientName,
    VisitStatus Status,
    List<(string Name, double Cost)> Procedures
);

public record DailyReportData(
    int Year,
    int Month,
    int Day,
    List<DailyVisitRow> Visits
);

public record DailyVisitRow(
    string DoctorName,
    DateTime ScheduledAt,
    string PatientName,
    VisitStatus Status,
    List<(string Name, double Cost)> Procedures
);

public record PatientReportData(
    string PatientName,
    string Pesel,
    DateOnly DateOfBirth,
    string Phone,
    string Email,
    string Insurance,
    List<PatientVisitRow> Visits
);

public record PatientVisitRow(
    DateTime ScheduledAt,
    string DoctorName,
    VisitStatus Status,
    List<(string Name, double Cost)> Procedures
);
