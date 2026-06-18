using AutoMapper;
using HMS.PatientRegistration.Application.Common.Helpers;
using HMS.PatientRegistration.Application.Common.Interfaces;
using HMS.PatientRegistration.Application.Common.Interfaces.Repositories;
using HMS.PatientRegistration.Application.Patients.DTOs;
using HMS.PatientRegistration.Domain.Entities;
using HMS.PatientRegistration.Domain.Enums;
using HMS.PatientRegistration.Domain.Exceptions;

namespace HMS.PatientRegistration.Application.Patients.Helpers;

internal static class PatientEntityFactory
{
    public static Patient CreateFromDto(CreatePatientDto dto, Guid id, string patientNumber, DateTime now, string? user)
    {
        var patient = new Patient
        {
            Id = id,
            PatientNumber = patientNumber,
            FirstName = dto.FirstName.Trim(),
            MiddleName = dto.MiddleName?.Trim(),
            LastName = dto.LastName.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Gender = EnumParser.ParseGender(dto.Gender),
            BloodGroup = EnumParser.ParseBloodGroup(dto.BloodGroup),
            NationalId = dto.NationalId,
            Nationality = dto.Nationality,
            Status = PatientStatus.Active,
            LegacyPatientId = dto.LegacyPatientId,
            Notes = dto.Notes,
            CreatedAt = now,
            CreatedBy = user,
        };

        ApplyChildCollections(patient, dto.Addresses, dto.Contacts, dto.Insurances, dto.EmergencyContacts);
        return patient;
    }

    public static void ApplyUpdate(Patient patient, UpdatePatientDto dto, DateTime now, string? user)
    {
        patient.FirstName = dto.FirstName.Trim();
        patient.MiddleName = dto.MiddleName?.Trim();
        patient.LastName = dto.LastName.Trim();
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = EnumParser.ParseGender(dto.Gender);
        patient.BloodGroup = EnumParser.ParseBloodGroup(dto.BloodGroup);
        patient.NationalId = dto.NationalId;
        patient.Nationality = dto.Nationality;
        patient.LegacyPatientId = dto.LegacyPatientId;
        patient.Notes = dto.Notes;
        patient.UpdatedAt = now;
        patient.UpdatedBy = user;

        patient.Addresses.Clear();
        patient.Contacts.Clear();
        patient.Insurances.Clear();
        patient.EmergencyContacts.Clear();

        ApplyChildCollections(patient, dto.Addresses, dto.Contacts, dto.Insurances, dto.EmergencyContacts);
    }

    private static void ApplyChildCollections(
        Patient patient,
        IReadOnlyList<CreatePatientAddressDto> addresses,
        IReadOnlyList<CreatePatientContactDto> contacts,
        IReadOnlyList<CreatePatientInsuranceDto> insurances,
        IReadOnlyList<CreateEmergencyContactDto> emergencyContacts)
    {
        foreach (var address in addresses)
        {
            patient.Addresses.Add(new PatientAddress
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                AddressType = EnumParser.ParseAddressType(address.AddressType),
                Line1 = address.Line1,
                Line2 = address.Line2,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsPrimary = address.IsPrimary,
            });
        }

        foreach (var contact in contacts)
        {
            patient.Contacts.Add(new PatientContact
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                ContactType = EnumParser.ParseContactType(contact.ContactType),
                Value = contact.Value,
                IsPrimary = contact.IsPrimary,
            });
        }

        foreach (var insurance in insurances)
        {
            patient.Insurances.Add(new PatientInsurance
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                ProviderName = insurance.ProviderName,
                PolicyNumber = insurance.PolicyNumber,
                GroupNumber = insurance.GroupNumber,
                ExpiryDate = insurance.ExpiryDate,
                IsPrimary = insurance.IsPrimary,
            });
        }

        foreach (var emergency in emergencyContacts)
        {
            patient.EmergencyContacts.Add(new EmergencyContact
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                Name = emergency.Name,
                Relationship = emergency.Relationship,
                Phone = emergency.Phone,
                Email = emergency.Email,
                IsPrimary = emergency.IsPrimary,
            });
        }
    }

    private static void ApplyChildCollections(
        Patient patient,
        IReadOnlyList<UpdatePatientAddressDto> addresses,
        IReadOnlyList<UpdatePatientContactDto> contacts,
        IReadOnlyList<UpdatePatientInsuranceDto> insurances,
        IReadOnlyList<UpdateEmergencyContactDto> emergencyContacts)
    {
        ApplyChildCollections(
            patient,
            addresses.Select(a => new CreatePatientAddressDto
            {
                AddressType = a.AddressType,
                Line1 = a.Line1,
                Line2 = a.Line2,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country,
                IsPrimary = a.IsPrimary,
            }).ToList(),
            contacts.Select(c => new CreatePatientContactDto
            {
                ContactType = c.ContactType,
                Value = c.Value,
                IsPrimary = c.IsPrimary,
            }).ToList(),
            insurances.Select(i => new CreatePatientInsuranceDto
            {
                ProviderName = i.ProviderName,
                PolicyNumber = i.PolicyNumber,
                GroupNumber = i.GroupNumber,
                ExpiryDate = i.ExpiryDate,
                IsPrimary = i.IsPrimary,
            }).ToList(),
            emergencyContacts.Select(e => new CreateEmergencyContactDto
            {
                Name = e.Name,
                Relationship = e.Relationship,
                Phone = e.Phone,
                Email = e.Email,
                IsPrimary = e.IsPrimary,
            }).ToList());
    }

    public static string? GetPrimaryPhone(CreatePatientDto dto) =>
        dto.Contacts.FirstOrDefault(c => c.IsPrimary)?.Value ?? dto.Contacts.FirstOrDefault()?.Value;

    public static string? GetPrimaryPhone(UpdatePatientDto dto) =>
        dto.Contacts.FirstOrDefault(c => c.IsPrimary)?.Value ?? dto.Contacts.FirstOrDefault()?.Value;
}
