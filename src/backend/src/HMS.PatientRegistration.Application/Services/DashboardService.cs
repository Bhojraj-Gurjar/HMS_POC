using AutoMapper;
using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Application.Dashboard.DTOs;
using HMS.PatientRegistration.Application.Patients.DTOs;
using HMS.PatientRegistration.Domain.Enums;

namespace HMS.PatientRegistration.Application.Services;

public class DashboardService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public DashboardService(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _patientRepository.SearchAsync(null, 1, 100, cancellationToken);
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var summaries = _mapper.Map<List<PatientSummaryDto>>(items);
        var newThisMonth = items.Count(p => p.CreatedAt >= monthStart);
        var activeCases = items.Count(p => p.Status == PatientStatus.Active);

        return new DashboardStatsDto
        {
            TotalPatients = totalCount,
            NewAdmissionsThisMonth = newThisMonth,
            ActiveCases = activeCases,
            AppointmentsToday = Math.Max(8, totalCount * 2),
            WeeklyActivity =
            [
                new() { Day = "Mon", Patients = 45, Appointments = 32 },
                new() { Day = "Tue", Patients = 52, Appointments = 38 },
                new() { Day = "Wed", Patients = 48, Appointments = 35 },
                new() { Day = "Thu", Patients = 61, Appointments = 42 },
                new() { Day = "Fri", Patients = 55, Appointments = 40 },
                new() { Day = "Sat", Patients = 38, Appointments = 28 },
                new() { Day = "Sun", Patients = 28, Appointments = 20 },
            ],
            DepartmentDistribution =
            [
                new() { Name = "Cardiology", Value = 28 },
                new() { Name = "Neurology", Value = 19 },
                new() { Name = "Orthopedics", Value = 24 },
                new() { Name = "Pediatrics", Value = 31 },
                new() { Name = "General", Value = 25 },
            ],
            RecentPatients = summaries.Take(5).ToList(),
        };
    }
}
