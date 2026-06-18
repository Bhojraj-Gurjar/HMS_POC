using System.Reflection;
using FluentValidation;
using HMS.PatientRegistration.Application.Common.Behaviors;
using HMS.PatientRegistration.Application.Common.Mappings;
using HMS.PatientRegistration.Application.Common.Interfaces.Services;
using HMS.PatientRegistration.Application.Common.Settings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.PatientRegistration.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<IPatientRegistrationService, Services.PatientRegistrationService>();
        services.AddScoped<ILegacyPatientRegistrationService, Services.LegacyPatientRegistrationService>();
        services.AddScoped<IDropdownService, Services.DropdownService>();
        services.AddScoped<Services.DashboardService>();

        return services;
    }
}
