using ClinicManager.Data;
using ClinicManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Pages.Debug.Test;

public class DbModel(AppDbContext db) : PageModel
{
    public string? Message { get; private set; }
    public bool IsError { get; private set; }
    public List<Patient> Patients { get; private set; } = [];

    public void OnGet() { }

    public async Task<IActionResult> OnPostInsertAsync()
    {
        try
        {
            var patient = new Patient
            {
                FirstName = "Test",
                LastName = $"Patient_{DateTime.UtcNow:HHmmss}",
                Pesel = "00000000000",
                InsuranceNumber = "TEST-0001"
            };
            db.Patients.Add(patient);
            await db.SaveChangesAsync();
            Message = $"Inserted patient with Id={patient.Id}.";
        }
        catch (Exception ex)
        {
            IsError = true;
            Message = $"Insert failed: {ex.Message}";
        }
        return Page();
    }

    public async Task<IActionResult> OnPostGetAsync()
    {
        try
        {
            Patients = await db.Patients.AsNoTracking().ToListAsync();
            Message = $"Found {Patients.Count} patient(s).";
        }
        catch (Exception ex)
        {
            IsError = true;
            Message = $"Query failed: {ex.Message}";
        }
        return Page();
    }
}
