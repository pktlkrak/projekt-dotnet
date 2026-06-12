using ClinicManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<Procedure> Procedures { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<Perscription> Perscriptions { get; set; }
    public DbSet<PerscriptionItem> PerscriptionItems { get; set; }
    public DbSet<MedicalFile> MedicalFiles { get; set; }
}