using HMS.PatientRegistration.Application.Common.Interfaces;

using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;

using HMS.PatientRegistration.Application.Common.Settings;

using HMS.PatientRegistration.Infrastructure.Persistence.Seed;
using HMS.PatientRegistration.Infrastructure.Persistence.StoredProcedures;

using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using HMS.PatientRegistration.Infrastructure.Identity;

using HMS.PatientRegistration.Infrastructure.Persistence;

using HMS.PatientRegistration.Infrastructure.Persistence.Repositories;

using HMS.PatientRegistration.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;



namespace HMS.PatientRegistration.Infrastructure;



public static class DependencyInjection

{

    public static IServiceCollection AddInfrastructure(

        this IServiceCollection services,

        IConfiguration configuration)

    {

        services.Configure<DataModeSettings>(configuration.GetSection(DataModeSettings.SectionName));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddHttpContextAccessor();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();



        services.AddScoped<IStoredProcedurePatientRepository, StoredProcedurePatientRepository>();

        var dataMode = configuration.GetSection(DataModeSettings.SectionName).Get<DataModeSettings>()

                       ?? new DataModeSettings();



        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var useDatabase = dataMode.IsDatabase;

        var mockFallbackActive = false;



        if (useDatabase && dataMode.AllowMockFallback && !SqlServerConnectionTester.CanConnect(connectionString))

        {

            useDatabase = false;

            mockFallbackActive = true;

        }



        services.AddSingleton<IRuntimeDataMode>(_ => new RuntimeDataMode

        {

            ConfiguredMode = dataMode.Mode,

            EffectiveMode = useDatabase ? dataMode.Mode : "Mock",

            IsMockFallbackActive = mockFallbackActive,

        });



        if (useDatabase)

        {

            services.AddDbContext<ApplicationDbContext>(options =>

                options.UseSqlServer(

                    connectionString,

                    sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));



            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IPatientRepository, PatientRepository>();

            services.AddScoped<IMasterDataRepository, MasterDataRepository>();

            services.AddHostedService<DatabaseInitializerHostedService>();

        }

        else

        {

            services.AddScoped<IUnitOfWork, MockUnitOfWork>();

            services.AddScoped<IPatientRepository, MockPatientRepository>();

            services.AddScoped<IMasterDataRepository, MockMasterDataRepository>();

        }



        return services;

    }

}



internal sealed class DatabaseInitializerHostedService : IHostedService

{

    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<DatabaseInitializerHostedService> _logger;



    public DatabaseInitializerHostedService(

        IServiceProvider serviceProvider,

        ILogger<DatabaseInitializerHostedService> logger)

    {

        _serviceProvider = serviceProvider;

        _logger = logger;

    }



    public async Task StartAsync(CancellationToken cancellationToken)

    {

        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var dataMode = scope.ServiceProvider.GetRequiredService<IOptions<DataModeSettings>>().Value;

        var runtimeDataMode = scope.ServiceProvider.GetRequiredService<IRuntimeDataMode>();



        if (!dataMode.IsDatabase || runtimeDataMode.IsMockFallbackActive)

        {

            return;

        }



        try

        {

            await context.Database.MigrateAsync(cancellationToken);

            await DatabaseSeeder.SeedAsync(context, cancellationToken);

            _logger.LogInformation("Database migrated and seed data applied.");

        }

        catch (Exception ex)

        {

            _logger.LogError(ex, "Database initialization failed.");

            throw;

        }

    }



    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}


