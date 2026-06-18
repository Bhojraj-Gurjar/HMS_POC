using HMS.PatientRegistration.Application.Common.Helpers;
using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.PatientRegistration.Infrastructure.Persistence.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Patients
            .Include(p => p.Addresses)
            .Include(p => p.Contacts)
            .Include(p => p.Insurances)
            .Include(p => p.EmergencyContacts)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Patient?> GetByPatientNumberAsync(string patientNumber, CancellationToken cancellationToken = default) =>
        await _context.Patients
            .FirstOrDefaultAsync(p => p.PatientNumber == patientNumber, cancellationToken);

    public async Task<(IReadOnlyList<Patient> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Patients
            .Include(p => p.Contacts)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.PatientNumber.ToLower().Contains(term) ||
                p.FirstName.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term) ||
                (p.NationalId != null && p.NationalId.ToLower().Contains(term)) ||
                (p.LegacyPatientId != null && p.LegacyPatientId.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<Patient> Items, int TotalCount)> SearchByCriteriaAsync(
        PatientSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Patients
            .Include(p => p.Contacts)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(criteria.MrNumber))
        {
            var term = criteria.MrNumber.Trim().ToLower();
            query = query.Where(p => p.PatientNumber.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(criteria.FirstName))
        {
            var term = criteria.FirstName.Trim().ToLower();
            query = query.Where(p => p.FirstName.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(criteria.LastName))
        {
            var term = criteria.LastName.Trim().ToLower();
            query = query.Where(p => p.LastName.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(criteria.CivilId))
        {
            var term = criteria.CivilId.Trim().ToLower();
            query = query.Where(p => p.NationalId != null && p.NationalId.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(criteria.MobileNumber))
        {
            var term = criteria.MobileNumber.Trim().ToLower();
            query = query.Where(p => p.Contacts.Any(c => c.Value.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Patient>> FindPotentialDuplicatesAsync(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string? nationalId,
        string? phone,
        CancellationToken cancellationToken = default)
    {
        var normalizedFirst = firstName.Trim().ToLower();
        var normalizedLast = lastName.Trim().ToLower();
        var normalizedNationalId = nationalId?.Trim().ToLower();
        var normalizedPhone = phone?.Trim().ToLower();

        var patients = await _context.Patients
            .Include(p => p.Contacts)
            .ToListAsync(cancellationToken);

        var matches = patients.Where(p =>
            (p.FirstName.ToLower() == normalizedFirst &&
             p.LastName.ToLower() == normalizedLast &&
             p.DateOfBirth == dateOfBirth) ||
            (!string.IsNullOrWhiteSpace(normalizedNationalId) &&
             p.NationalId != null &&
             p.NationalId.ToLower() == normalizedNationalId) ||
            (!string.IsNullOrWhiteSpace(normalizedPhone) &&
             p.Contacts.Any(c => c.Value.ToLower() == normalizedPhone)))
            .ToList();

        return matches;
    }

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddAsync(patient, cancellationToken);
    }

    public Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _context.Patients.Update(patient);
        return Task.CompletedTask;
    }

    public Task SoftDeleteAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _context.Patients.Update(patient);
        return Task.CompletedTask;
    }

    public async Task<string> GenerateNextPatientNumberAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"MRN-{year}-";

        var lastNumber = await _context.Patients
            .Where(p => p.PatientNumber.StartsWith(prefix))
            .OrderByDescending(p => p.PatientNumber)
            .Select(p => p.PatientNumber)
            .FirstOrDefaultAsync(cancellationToken);

        var sequence = 1;
        if (!string.IsNullOrEmpty(lastNumber))
        {
            var parts = lastNumber.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out var parsed))
            {
                sequence = parsed + 1;
            }
        }

        return $"{prefix}{sequence:D6}";
    }
}
