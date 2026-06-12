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
    }
}
