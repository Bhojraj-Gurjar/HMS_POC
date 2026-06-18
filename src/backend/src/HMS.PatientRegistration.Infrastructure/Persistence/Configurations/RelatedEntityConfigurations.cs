using HMS.PatientRegistration.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.PatientRegistration.Infrastructure.Persistence.Configurations;

public class PatientAddressConfiguration : IEntityTypeConfiguration<PatientAddress>
{
    public void Configure(EntityTypeBuilder<PatientAddress> builder)
    {
        builder.ToTable("PatientAddresses");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Line1).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Line2).HasMaxLength(200);
        builder.Property(a => a.City).HasMaxLength(100).IsRequired();
        builder.Property(a => a.State).HasMaxLength(100);
        builder.Property(a => a.PostalCode).HasMaxLength(20);
        builder.Property(a => a.Country).HasMaxLength(100).IsRequired();
    }
}

public class PatientContactConfiguration : IEntityTypeConfiguration<PatientContact>
{
    public void Configure(EntityTypeBuilder<PatientContact> builder)
    {
        builder.ToTable("PatientContacts");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Value).HasMaxLength(200).IsRequired();
    }
}

public class PatientInsuranceConfiguration : IEntityTypeConfiguration<PatientInsurance>
{
    public void Configure(EntityTypeBuilder<PatientInsurance> builder)
    {
        builder.ToTable("PatientInsurances");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ProviderName).HasMaxLength(200).IsRequired();
        builder.Property(i => i.PolicyNumber).HasMaxLength(100).IsRequired();
        builder.Property(i => i.GroupNumber).HasMaxLength(100);
    }
}

public class EmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmergencyContact> builder)
    {
        builder.ToTable("EmergencyContacts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Relationship).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(200);
    }
}

public class MasterDataItemConfiguration : IEntityTypeConfiguration<MasterDataItem>
{
    public void Configure(EntityTypeBuilder<MasterDataItem> builder)
    {
        builder.ToTable("MasterDataItems");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Code).HasMaxLength(50).IsRequired();
        builder.Property(m => m.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(m => m.ParentCode).HasMaxLength(50);
        builder.HasIndex(m => new { m.Type, m.Code }).IsUnique();
    }
}
