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
    public DbSet<ProcedureRef> ProcedureRefs { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<Perscription> Perscriptions { get; set; }
    public DbSet<PerscriptionItem> PerscriptionItems { get; set; }
    public DbSet<MedicalFile> MedicalFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProcedureRef>()
            .HasOne(pr => pr.Visit)
            .WithMany(v => v.Procedures)
            .HasForeignKey(pr => pr.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProcedureRef>()
            .HasOne(pr => pr.Procedure)
            .WithMany()
            .HasForeignKey(pr => pr.ProcedureId)
            .OnDelete(DeleteBehavior.Restrict);

        // Microsoft.Data.SqlClient 6.1.1 throws InvalidCastException when reading
        // a 'date' column directly as DateOnly (GetFieldValue<DateOnly>). Reading
        // it as DateTime and converting in memory avoids that buggy code path.
        modelBuilder.Entity<Patient>()
            .Property(p => p.DateOfBirth)
            .HasConversion(
                d => d.ToDateTime(TimeOnly.MinValue),
                dt => DateOnly.FromDateTime(dt))
            .HasColumnType("date");
    }
}
