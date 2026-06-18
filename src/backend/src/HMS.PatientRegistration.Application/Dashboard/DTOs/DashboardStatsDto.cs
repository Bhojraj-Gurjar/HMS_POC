namespace HMS.PatientRegistration.Application.Dashboard.DTOs;

public class DashboardStatsDto
{
    public int TotalPatients { get; set; }
    public int NewAdmissionsThisMonth { get; set; }
    public int ActiveCases { get; set; }
    public int AppointmentsToday { get; set; }
    public IReadOnlyList<WeeklyActivityDto> WeeklyActivity { get; set; } = Array.Empty<WeeklyActivityDto>();
    public IReadOnlyList<DepartmentDistributionDto> DepartmentDistribution { get; set; } =
        Array.Empty<DepartmentDistributionDto>();
    public IReadOnlyList<Patients.DTOs.PatientSummaryDto> RecentPatients { get; set; } =
        Array.Empty<Patients.DTOs.PatientSummaryDto>();
}

public class WeeklyActivityDto
{
    public string Day { get; set; } = string.Empty;
    public int Patients { get; set; }
    public int Appointments { get; set; }
}

public class DepartmentDistributionDto
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}
