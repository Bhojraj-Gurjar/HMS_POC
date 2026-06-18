using System.Text;
using HMS.PatientRegistration.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace HMS.PatientRegistration.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HMS Patient Registration API",
                Version = "v1",
                Description =
                    "Hospital Management System — Patient Registration Migration POC.\n\n" +
                    "**Base path:** `/api`\n\n" +
                    "**Response envelope:** All endpoints return `ApiResponse<T>` with `success`, `message`, `data`, `errors`, and `correlationId`.\n\n" +
                    "### Endpoint groups\n" +
                    "| Group | Routes |\n" +
                    "|-------|--------|\n" +
                    "| Health | `GET /health` |\n" +
                    "| Auth | `POST /auth/login` |\n" +
                    "| Dashboard | `GET /dashboard/stats` |\n" +
                    "| Patients | `GET/POST/PUT/DELETE /patients`, `POST /patients/duplicate-check` |\n" +
                    "| Patient Registration | `POST/PUT/GET /patient-registration`, `POST /patient-registration/search` |\n" +
                    "| Master Data | `GET /masterdata/{type}`, `GET /dropdowns/{type}` |\n" +
                    "| Legacy Adapters | `POST /patientregistration/*`, `POST /CommonDropdown/Fetch` |\n\n" +
                    "**Swagger UI:** `/swagger` (Development environment)\n\n" +
                    "**Auth:** JWT Bearer is configured; POC controllers use `[AllowAnonymous]`."
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(ServiceCollectionExtensions).Assembly.GetName().Name}.xml");
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            options.TagActionsBy(api =>
            {
                if (api.GroupName == "legacy")
                {
                    return ["Legacy Adapters"];
                }

                var controller = api.ActionDescriptor.RouteValues.TryGetValue("controller", out var name)
                    ? name
                    : "API";
                return [controller ?? "API"];
            });

            options.OrderActionsBy(apiDesc => $"{apiDesc.GroupName}_{apiDesc.RelativePath}");

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Placeholder token for POC.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddCors(options =>
        {
            var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            options.AddPolicy("DefaultCors", policy =>
            {
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        AddJwtAuthenticationPlaceholder(services, configuration);

        return services;
    }

    private static void AddJwtAuthenticationPlaceholder(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? Environments.Production;
        jwtSettings.Validate(string.Equals(environment, Environments.Development, StringComparison.OrdinalIgnoreCase));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("Staff", policy => policy.RequireRole("Receptionist", "Admin"));
        });
    }
}

public static class WebApplicationExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseMiddleware<Middleware.CorrelationIdMiddleware>();
        app.UseMiddleware<Middleware.RequestLoggingMiddleware>();
        app.UseMiddleware<Middleware.ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS Patient Registration API v1");
                options.DocumentTitle = "HMS Patient Registration API";
                options.DisplayRequestDuration();
                options.EnableTryItOutByDefault();
            });
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }

        app.UseCors("DefaultCors");

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
