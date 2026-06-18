using HMS.PatientRegistration.Application.Common.Interfaces;
using HMS.PatientRegistration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.PatientRegistration.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientAddress> PatientAddresses => Set<PatientAddress>();
    public DbSet<PatientContact> PatientContacts => Set<PatientContact>();
    public DbSet<PatientInsurance> PatientInsurances => Set<PatientInsurance>();
    public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();
    public DbSet<MasterDataItem> MasterDataItems => Set<MasterDataItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        base.SaveChangesAsync(cancellationToken);
}
