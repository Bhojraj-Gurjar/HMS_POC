using HMS.PatientRegistration.Domain.Enums;

namespace HMS.PatientRegistration.Application.Common.Helpers;

public static class EnumParser
{
    public static Gender ParseGender(string value) =>
        Enum.TryParse<Gender>(value, ignoreCase: true, out var result) ? result : Gender.Unknown;

    public static BloodGroup ParseBloodGroup(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return BloodGroup.Unknown;
        }

        var normalized = value.Trim().Replace(" ", string.Empty) switch
        {
            "A+" => nameof(BloodGroup.APositive),
            "A-" => nameof(BloodGroup.ANegative),
            "B+" => nameof(BloodGroup.BPositive),
            "B-" => nameof(BloodGroup.BNegative),
            "AB+" => nameof(BloodGroup.ABPositive),
            "AB-" => nameof(BloodGroup.ABNegative),
            "O+" => nameof(BloodGroup.OPositive),
            "O-" => nameof(BloodGroup.ONegative),
            _ => value.Trim().Replace(" ", string.Empty),
        };

        return Enum.TryParse<BloodGroup>(normalized, ignoreCase: true, out var result)
            ? result
            : BloodGroup.Unknown;
    }

    public static PatientStatus ParsePatientStatus(string value) =>
        Enum.TryParse<PatientStatus>(value, ignoreCase: true, out var result) ? result : PatientStatus.Active;

    public static AddressType ParseAddressType(string value) =>
        Enum.TryParse<AddressType>(value, ignoreCase: true, out var result) ? result : AddressType.Home;

    public static ContactType ParseContactType(string value) =>
        Enum.TryParse<ContactType>(value, ignoreCase: true, out var result) ? result : ContactType.Mobile;

    public static MasterDataType ParseMasterDataType(string value)
    {
        var normalized = value.Trim().Replace(" ", string.Empty);

        if (normalized.Equals("PatientTitle", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("Title", StringComparison.OrdinalIgnoreCase))
        {
            return MasterDataType.Prefix;
        }

        if (normalized.Equals("IncomeCategory", StringComparison.OrdinalIgnoreCase))
        {
            return MasterDataType.IncomeCategory;
        }

        return Enum.TryParse<MasterDataType>(normalized, ignoreCase: true, out var result)
            ? result
            : throw new ArgumentException($"Invalid master data type: {value}");
    }
}
