using HMS.PatientRegistration.Domain.Entities;
using HMS.PatientRegistration.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

namespace HMS.PatientRegistration.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        await SeedMasterDataAsync(context, cancellationToken);
        await SeedPatientsAsync(context, cancellationToken);
    }

    public static async Task SeedMasterDataAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.MasterDataItems.AnyAsync(cancellationToken))
        {
            return;
        }

        await context.MasterDataItems.AddRangeAsync(MockSeedData.CreateMasterDataItems(), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public static async Task SeedPatientsAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Patients.AnyAsync(cancellationToken))
        {
            return;
        }

        await context.Patients.AddRangeAsync(MockSeedData.CreatePatients(), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
