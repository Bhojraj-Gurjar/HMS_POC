using HMS.PatientRegistration.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.PatientRegistration.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PatientNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(p => p.PatientNumber).IsUnique();
        builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.MiddleName).HasMaxLength(100);
        builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.NationalId).HasMaxLength(50);
        builder.Property(p => p.Nationality).HasMaxLength(100);
        builder.Property(p => p.LegacyPatientId).HasMaxLength(100);
        builder.HasIndex(p => p.LegacyPatientId);
        builder.Property(p => p.Notes).HasMaxLength(2000);
        builder.Property(p => p.CreatedBy).HasMaxLength(100);
        builder.Property(p => p.UpdatedBy).HasMaxLength(100);
        builder.Property(p => p.DeletedBy).HasMaxLength(100);
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasMany(p => p.Addresses).WithOne(a => a.Patient).HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Contacts).WithOne(c => c.Patient).HasForeignKey(c => c.PatientId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Insurances).WithOne(i => i.Patient).HasForeignKey(i => i.PatientId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.EmergencyContacts).WithOne(e => e.Patient).HasForeignKey(e => e.PatientId).OnDelete(DeleteBehavior.Cascade);
    }
}
