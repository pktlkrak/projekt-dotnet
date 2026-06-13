using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class ReportService(AppDbContext context, UserManager<ApplicationUser> userManager) : IReportService
{
    public Task<List<ApplicationUser>> GetDoctorsAsync() =>
        userManager.GetUsersInRoleAsync("Doctor")
            .ContinueWith(t => t.Result.OrderBy(d => d.LastName).ThenBy(d => d.FirstName).ToList());

    public async Task<MonthlyReportData?> GetMonthlyReportAsync(int year, int month, string doctorId)
    {
        var doctor = await userManager.FindByIdAsync(doctorId);
        if (doctor == null) return null;

        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);

        var visits = await context.Visits
            .Include(v => v.Patient)
            .Include(v => v.Procedures).ThenInclude(pr => pr.Procedure)
            .Where(v => v.DoctorId == doctorId
                     && v.ScheduledAt >= from
                     && v.ScheduledAt < to
                     && v.Status != VisitStatus.Cancelled)
            .OrderBy(v => v.ScheduledAt)
            .ToListAsync();

        var rows = visits.Select(v => new MonthlyVisitRow(
            v.ScheduledAt,
            $"{v.Patient?.LastName} {v.Patient?.FirstName}",
            v.Status,
            v.Procedures.Select(pr => (pr.Procedure?.Name ?? "?", pr.Cost)).ToList()
        )).ToList();

        return new MonthlyReportData($"{doctor.LastName} {doctor.FirstName}", year, month, rows);
    }

    public async Task<PatientReportData?> GetPatientReportAsync(int patientId)
    {
        var patient = await context.Patients.FindAsync(patientId);
        if (patient == null) return null;

        var visits = await context.Visits
            .Include(v => v.Doctor)
            .Include(v => v.Procedures).ThenInclude(pr => pr.Procedure)
            .Where(v => v.PatientId == patientId)
            .OrderBy(v => v.ScheduledAt)
            .ToListAsync();

        var rows = visits.Select(v => new PatientVisitRow(
            v.ScheduledAt,
            $"{v.Doctor?.LastName} {v.Doctor?.FirstName}",
            v.Status,
            v.Procedures.Select(pr => (pr.Procedure?.Name ?? "?", pr.Cost)).ToList()
        )).ToList();

        return new PatientReportData(
            $"{patient.LastName} {patient.FirstName}",
            patient.Pesel,
            patient.DateOfBirth,
            patient.PhoneNumber,
            patient.Email,
            patient.InsuranceNumber,
            rows
        );
    }
}
