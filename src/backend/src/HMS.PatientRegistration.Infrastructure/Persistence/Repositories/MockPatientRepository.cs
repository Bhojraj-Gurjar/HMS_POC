using System.Collections.Concurrent;
using HMS.PatientRegistration.Application.Common.Helpers;
using HMS.PatientRegistration.Application.Common.Interfaces;
using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Domain.Entities;
using HMS.PatientRegistration.Infrastructure.Persistence.Seed;

namespace HMS.PatientRegistration.Infrastructure.Persistence.Repositories;

/// <summary>
/// In-memory repository used when DataMode is set to Mock.
/// Thread-safe for concurrent API requests during POC/demo.
/// </summary>
public class MockPatientRepository : IPatientRepository
{
    private static readonly ConcurrentDictionary<Guid, Patient> Store = new();
    private static int _sequence = 5;
    private readonly IDateTimeProvider _dateTimeProvider;

    static MockPatientRepository()
    {
        SeedSampleData();
    }

    public MockPatientRepository(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Store.TryGetValue(id, out var patient);
        return Task.FromResult(patient is { IsDeleted: false } ? Clone(patient) : null);
    }

    public Task<Patient?> GetByPatientNumberAsync(string patientNumber, CancellationToken cancellationToken = default)
    {
        var patient = Store.Values.FirstOrDefault(p =>
            !p.IsDeleted && p.PatientNumber.Equals(patientNumber, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(patient is null ? null : Clone(patient));
    }

    public Task<(IReadOnlyList<Patient> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Store.Values.Where(p => !p.IsDeleted).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.PatientNumber.ToLower().Contains(term) ||
                p.FirstName.ToLower().Contains(term) ||
                p.LastName.ToLower().Contains(term) ||
                (p.NationalId?.ToLower().Contains(term) ?? false) ||
                p.Contacts.Any(c => c.Value.ToLower().Contains(term)));
        }

        var list = query.OrderByDescending(p => p.CreatedAt).ToList();
        var total = list.Count;
        var page = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(Clone).ToList();
        return Task.FromResult<(IReadOnlyList<Patient>, int)>((page, total));
    }

    public Task<(IReadOnlyList<Patient> Items, int TotalCount)> SearchByCriteriaAsync(
        PatientSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = Store.Values
            .Where(p => !p.IsDeleted)
            .Where(p => PatientSearchCriteriaMapper.Matches(p, criteria))
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        var total = query.Count;
        var page = query
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .Select(Clone)
            .ToList();

        return Task.FromResult<(IReadOnlyList<Patient>, int)>((page, total));
    }

    public Task<IReadOnlyList<Patient>> FindPotentialDuplicatesAsync(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string? nationalId,
        string? phone,
        CancellationToken cancellationToken = default)
    {
        var matches = Store.Values.Where(p => !p.IsDeleted).Where(p =>
            (p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
             p.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase) &&
             p.DateOfBirth == dateOfBirth) ||
            (!string.IsNullOrWhiteSpace(nationalId) &&
             p.NationalId != null &&
             p.NationalId.Equals(nationalId, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(phone) &&
             p.Contacts.Any(c => c.Value.Equals(phone, StringComparison.OrdinalIgnoreCase))));

        return Task.FromResult<IReadOnlyList<Patient>>(matches.Select(Clone).ToList());
    }

    public Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        Store[patient.Id] = Clone(patient);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        Store[patient.Id] = Clone(patient);
        return Task.CompletedTask;
    }

    public Task SoftDeleteAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        Store[patient.Id] = Clone(patient);
        return Task.CompletedTask;
    }

    public Task<string> GenerateNextPatientNumberAsync(CancellationToken cancellationToken = default)
    {
        var year = _dateTimeProvider.UtcNow.Year;
        var number = Interlocked.Increment(ref _sequence);
        return Task.FromResult($"MRN-{year}-{number:D6}");
    }

    private static void SeedSampleData()
    {
        foreach (var patient in MockSeedData.CreatePatients())
        {
            Store[patient.Id] = Clone(patient);
        }
    }

    private static Patient Clone(Patient source)
    {
        var clone = new Patient
        {
            Id = source.Id,
            PatientNumber = source.PatientNumber,
            FirstName = source.FirstName,
            MiddleName = source.MiddleName,
            LastName = source.LastName,
            DateOfBirth = source.DateOfBirth,
            Gender = source.Gender,
            BloodGroup = source.BloodGroup,
            NationalId = source.NationalId,
            Nationality = source.Nationality,
            Status = source.Status,
            LegacyPatientId = source.LegacyPatientId,
            Notes = source.Notes,
            CreatedAt = source.CreatedAt,
            CreatedBy = source.CreatedBy,
            UpdatedAt = source.UpdatedAt,
            UpdatedBy = source.UpdatedBy,
            IsDeleted = source.IsDeleted,
            DeletedAt = source.DeletedAt,
            DeletedBy = source.DeletedBy
        };

        foreach (var address in source.Addresses)
        {
            clone.Addresses.Add(new PatientAddress
            {
                Id = address.Id,
                PatientId = address.PatientId,
                AddressType = address.AddressType,
                Line1 = address.Line1,
                Line2 = address.Line2,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsPrimary = address.IsPrimary
            });
        }

        foreach (var contact in source.Contacts)
        {
            clone.Contacts.Add(new PatientContact
            {
                Id = contact.Id,
                PatientId = contact.PatientId,
                ContactType = contact.ContactType,
                Value = contact.Value,
                IsPrimary = contact.IsPrimary
            });
        }

        foreach (var insurance in source.Insurances)
        {
            clone.Insurances.Add(new PatientInsurance
            {
                Id = insurance.Id,
                PatientId = insurance.PatientId,
                ProviderName = insurance.ProviderName,
                PolicyNumber = insurance.PolicyNumber,
                GroupNumber = insurance.GroupNumber,
                ExpiryDate = insurance.ExpiryDate,
                IsPrimary = insurance.IsPrimary
            });
        }

        foreach (var emergency in source.EmergencyContacts)
        {
            clone.EmergencyContacts.Add(new EmergencyContact
            {
                Id = emergency.Id,
                PatientId = emergency.PatientId,
                Name = emergency.Name,
                Relationship = emergency.Relationship,
                Phone = emergency.Phone,
                Email = emergency.Email,
                IsPrimary = emergency.IsPrimary
            });
        }

        return clone;
    }
}
