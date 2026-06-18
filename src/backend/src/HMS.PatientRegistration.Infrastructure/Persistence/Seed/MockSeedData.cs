using HMS.PatientRegistration.Domain.Entities;
using HMS.PatientRegistration.Domain.Enums;

namespace HMS.PatientRegistration.Infrastructure.Persistence.Seed;

/// <summary>
/// Shared seed data for Mock repositories and SQL Server initial seeding.
/// </summary>
public static class MockSeedData
{
    public static IReadOnlyList<MasterDataItem> CreateMasterDataItems()
    {
        var items = new List<MasterDataItem>();

        Add(items, MasterDataType.Prefix, [
            ("Mr", "Mr"),
            ("Mrs", "Mrs"),
            ("Ms", "Ms"),
            ("Miss", "Miss"),
            ("Dr", "Dr"),
            ("Master", "Master"),
            ("Rev", "Rev"),
        ]);

        Add(items, MasterDataType.Gender, [
            ("Male", "Male"),
            ("Female", "Female"),
            ("Other", "Other"),
        ]);

        Add(items, MasterDataType.MaritalStatus, [
            ("Single", "Single"),
            ("Married", "Married"),
            ("Divorced", "Divorced"),
            ("Widowed", "Widowed"),
            ("Separated", "Separated"),
        ]);

        Add(items, MasterDataType.BloodGroup, [
            ("APositive", "A Positive"),
            ("ANegative", "A Negative"),
            ("BPositive", "B Positive"),
            ("BNegative", "B Negative"),
            ("ABPositive", "AB Positive"),
            ("ABNegative", "AB Negative"),
            ("OPositive", "O Positive"),
            ("ONegative", "O Negative"),
        ]);

        Add(items, MasterDataType.Nationality, [
            ("KW", "Kuwaiti"),
            ("SA", "Saudi"),
            ("AE", "Emirati"),
            ("EG", "Egyptian"),
            ("IN", "Indian"),
            ("PK", "Pakistani"),
            ("US", "American"),
            ("UK", "British"),
        ]);

        Add(items, MasterDataType.Race, [
            ("Asian", "Asian"),
            ("Black", "Black / African"),
            ("White", "White / Caucasian"),
            ("Hispanic", "Hispanic / Latino"),
            ("MiddleEastern", "Middle Eastern"),
            ("Mixed", "Mixed"),
            ("Other", "Other"),
        ]);

        Add(items, MasterDataType.Religion, [
            ("Islam", "Islam"),
            ("Christianity", "Christianity"),
            ("Hinduism", "Hinduism"),
            ("Buddhism", "Buddhism"),
            ("Judaism", "Judaism"),
            ("None", "None"),
            ("Other", "Other"),
        ]);

        Add(items, MasterDataType.Language, [
            ("Arabic", "Arabic"),
            ("English", "English"),
            ("Hindi", "Hindi"),
            ("Urdu", "Urdu"),
            ("Filipino", "Filipino"),
            ("French", "French"),
        ]);

        Add(items, MasterDataType.Country, [
            ("KW", "Kuwait"),
            ("SA", "Saudi Arabia"),
            ("AE", "United Arab Emirates"),
            ("US", "United States"),
            ("UK", "United Kingdom"),
            ("IN", "India"),
            ("EG", "Egypt"),
        ]);

        var stateEntries = new List<(string Code, string DisplayName, string ParentCode)>
        {
            ("KW-C", "Kuwait City", "KW"),
            ("SA-01", "Riyadh Province", "SA"),
            ("AE-DU", "Dubai", "AE"),
            ("US-IL", "Illinois", "US"),
            ("US-TX", "Texas", "US"),
            ("US-MA", "Massachusetts", "US"),
            ("UK-ENG", "England", "UK"),
            ("EG-C", "Cairo", "EG"),
        };
        stateEntries.AddRange(IndiaGeographySeed.States);
        AddHierarchical(items, MasterDataType.State, stateEntries);

        var cityEntries = new List<(string Code, string DisplayName, string ParentCode)>
        {
            ("KW-KC", "Kuwait City", "KW-C"),
            ("SA-RY", "Riyadh", "SA-01"),
            ("AE-DXB", "Dubai", "AE-DU"),
            ("US-SPF", "Springfield", "US-IL"),
            ("US-CHI", "Chicago", "US-IL"),
            ("US-AUS", "Austin", "US-TX"),
            ("US-BOS", "Boston", "US-MA"),
            ("UK-LON", "London", "UK-ENG"),
            ("EG-CAI", "Cairo", "EG-C"),
        };
        cityEntries.AddRange(IndiaGeographySeed.Cities);
        AddHierarchical(items, MasterDataType.City, cityEntries);

        AddHierarchical(items, MasterDataType.Area, [
            ("KW-SAL", "Salmiya", "KW-KC"),
            ("KW-HAW", "Hawally", "KW-KC"),
            ("KW-JAB", "Jabriya", "KW-KC"),
            ("AE-DER", "Deira", "AE-DXB"),
            ("AE-DTN", "Downtown Dubai", "AE-DXB"),
            ("US-DTN", "Downtown", "US-CHI"),
            ("US-NOR", "North Austin", "US-AUS"),
            ("UK-WST", "Westminster", "UK-LON"),
            ("IN-MH-AND", "Andheri", "IN-MH-MUM"),
            ("IN-MH-BKC", "Bandra Kurla Complex", "IN-MH-MUM"),
            ("IN-MH-KOR", "Koregaon Park", "IN-MH-PUN"),
            ("IN-DL-CP", "Connaught Place", "IN-DL-NDL"),
            ("IN-KA-IND", "Indiranagar", "IN-KA-BLR"),
            ("IN-TN-EGM", "Egmore", "IN-TN-CHE"),
            ("IN-UP-GOM", "Gomti Nagar", "IN-UP-LKO"),
            ("IN-GJ-SG", "Satellite", "IN-GJ-AMD"),
            ("EG-ZAM", "Zamalek", "EG-CAI"),
        ]);

        Add(items, MasterDataType.Occupation, [
            ("Nurse", "Registered Nurse"),
            ("Engineer", "Engineer"),
            ("Teacher", "Teacher"),
            ("Physician", "Physician"),
            ("Student", "Student"),
            ("Executive", "Business Executive"),
            ("Retired", "Retired"),
            ("Homemaker", "Homemaker"),
        ]);

        Add(items, MasterDataType.Company, [
            ("SGH", "Springfield General Hospital"),
            ("TC", "Tech Corp International"),
            ("CS", "City Schools District"),
            ("BMC", "Boston Medical Center"),
            ("KOC", "Kuwait Oil Company"),
            ("STC", "Saudi Telecom"),
            ("SELF", "Self Employed"),
            ("NA", "Not Applicable"),
        ]);

        Add(items, MasterDataType.Profession, [
            ("Medicine", "Medicine"),
            ("Engineering", "Engineering"),
            ("Education", "Education"),
            ("Healthcare", "Healthcare"),
            ("IT", "Information Technology"),
            ("Finance", "Finance"),
            ("Legal", "Legal"),
            ("Other", "Other"),
        ]);

        Add(items, MasterDataType.IncomeCategory, [
            ("Low", "Low Income"),
            ("LowerMiddle", "Lower Middle Income"),
            ("Middle", "Middle Income"),
            ("UpperMiddle", "Upper Middle Income"),
            ("High", "High Income"),
        ]);

        Add(items, MasterDataType.Relationship, [
            ("Spouse", "Spouse"),
            ("Parent", "Parent"),
            ("Sibling", "Sibling"),
            ("Child", "Child"),
            ("Guardian", "Guardian"),
            ("Friend", "Friend"),
        ]);

        return items;
    }

    public static IReadOnlyList<Patient> CreatePatients()
    {
        return
        [
            BuildJaneDoe(),
            BuildJohnSmith(),
            BuildAishaKhan(),
            BuildRobertChen(),
            BuildEmilyBrown(),
        ];
    }

    private static Patient BuildJaneDoe()
    {
        var id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var patient = new Patient
        {
            Id = id,
            PatientNumber = "MRN-2026-000001",
            FirstName = "Jane",
            MiddleName = "Marie",
            LastName = "Doe",
            DateOfBirth = new DateOnly(1990, 5, 15),
            Gender = Gender.Female,
            BloodGroup = BloodGroup.OPositive,
            NationalId = "CIV-123456",
            Nationality = "US",
            Status = PatientStatus.Active,
            LegacyPatientId = "LEG-001",
            Notes = "Regular outpatient | Marital Status: Married | Religion: Christianity | Language: English",
            CreatedAt = new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc),
            CreatedBy = "seed",
        };

        AddContacts(patient,
            Contact(id, ContactType.Mobile, "+1-555-0100", true),
            Contact(id, ContactType.Email, "jane.doe@email.com", false),
            Contact(id, ContactType.WorkPhone, "+1-555-0101", false));

        patient.Addresses.Add(Address(id, AddressType.Home, "123 Main St", "Springfield", "IL", "62701", "US", true));

        patient.Insurances.Add(new PatientInsurance
        {
            Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
            PatientId = id,
            ProviderName = "HealthCare Plus",
            PolicyNumber = "POL-7788",
            GroupNumber = "GRP-22",
            ExpiryDate = new DateOnly(2027, 12, 31),
            IsPrimary = true,
        });

        patient.EmergencyContacts.Add(new EmergencyContact
        {
            Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
            PatientId = id,
            Name = "John Doe",
            Relationship = "Spouse",
            Phone = "+1-555-0102",
            Email = "john.doe@email.com",
            IsPrimary = true,
        });

        return patient;
    }

    private static Patient BuildJohnSmith()
    {
        var id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var patient = new Patient
        {
            Id = id,
            PatientNumber = "MRN-2026-000002",
            FirstName = "John",
            LastName = "Smith",
            DateOfBirth = new DateOnly(1985, 3, 20),
            Gender = Gender.Male,
            BloodGroup = BloodGroup.APositive,
            NationalId = "CIV-789012",
            Nationality = "US",
            Status = PatientStatus.Active,
            Notes = "Marital Status: Single | Occupation: Engineer | Company: Tech Corp International",
            CreatedAt = new DateTime(2026, 2, 1, 10, 30, 0, DateTimeKind.Utc),
            CreatedBy = "seed",
        };

        AddContacts(patient,
            Contact(id, ContactType.Mobile, "+1-555-0199", true),
            Contact(id, ContactType.Email, "john.smith@email.com", false));

        patient.Addresses.Add(Address(id, AddressType.Home, "456 Oak Ave", "Chicago", "IL", "60601", "US", true));

        return patient;
    }

    private static Patient BuildAishaKhan()
    {
        var id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var patient = new Patient
        {
            Id = id,
            PatientNumber = "MRN-2026-000003",
            FirstName = "Aisha",
            LastName = "Khan",
            DateOfBirth = new DateOnly(1998, 11, 2),
            Gender = Gender.Female,
            BloodGroup = BloodGroup.BPositive,
            NationalId = "CIV-456789",
            Nationality = "UK",
            Status = PatientStatus.Active,
            Notes = "Marital Status: Single | Religion: Islam | Language: Urdu | Area: Westminster",
            CreatedAt = new DateTime(2026, 3, 5, 14, 15, 0, DateTimeKind.Utc),
            CreatedBy = "seed",
        };

        AddContacts(patient,
            Contact(id, ContactType.Mobile, "+44-7700-900123", true),
            Contact(id, ContactType.Email, "aisha.khan@email.com", false));

        patient.Addresses.Add(Address(id, AddressType.Home, "12 Baker Street", "London", "England", "NW1 6XE", "UK", true));

        patient.EmergencyContacts.Add(new EmergencyContact
        {
            Id = Guid.Parse("b3333333-3333-3333-3333-333333333333"),
            PatientId = id,
            Name = "Fatima Khan",
            Relationship = "Parent",
            Phone = "+44-7700-900456",
            IsPrimary = true,
        });

        return patient;
    }

    private static Patient BuildRobertChen()
    {
        var id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var patient = new Patient
        {
            Id = id,
            PatientNumber = "MRN-2026-000004",
            FirstName = "Robert",
            MiddleName = "James",
            LastName = "Chen",
            DateOfBirth = new DateOnly(1972, 7, 30),
            Gender = Gender.Male,
            BloodGroup = BloodGroup.ABNegative,
            NationalId = "CIV-334455",
            Nationality = "US",
            Status = PatientStatus.Active,
            LegacyPatientId = "EMP-5500",
            Notes = "Profession: Medicine | Income Category: High Income | Company: Boston Medical Center",
            CreatedAt = new DateTime(2026, 3, 12, 9, 0, 0, DateTimeKind.Utc),
            CreatedBy = "seed",
        };

        AddContacts(patient,
            Contact(id, ContactType.Mobile, "+1-555-0244", true),
            Contact(id, ContactType.WorkPhone, "+1-555-0245", false),
            Contact(id, ContactType.Email, "r.chen@bostonmed.org", false));

        patient.Addresses.Add(Address(id, AddressType.Work, "900 Medical Plaza", "Boston", "MA", "02115", "US", true));

        return patient;
    }

    private static Patient BuildEmilyBrown()
    {
        var id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var patient = new Patient
        {
            Id = id,
            PatientNumber = "MRN-2026-000005",
            FirstName = "Emily",
            LastName = "Brown",
            DateOfBirth = new DateOnly(2010, 12, 1),
            Gender = Gender.Female,
            BloodGroup = BloodGroup.OPositive,
            NationalId = "CIV-667788",
            Nationality = "US",
            Status = PatientStatus.Active,
            Notes = "Pediatric patient | Area: North Austin | Guardian on file",
            CreatedAt = new DateTime(2026, 4, 1, 11, 20, 0, DateTimeKind.Utc),
            CreatedBy = "seed",
        };

        patient.Contacts.Add(Contact(id, ContactType.Mobile, "+1-555-0311", true));

        patient.Addresses.Add(Address(id, AddressType.Home, "78 Pine Road", "Austin", "TX", "78701", "US", true));

        patient.EmergencyContacts.Add(new EmergencyContact
        {
            Id = Guid.Parse("b5555555-5555-5555-5555-555555555555"),
            PatientId = id,
            Name = "Sarah Brown",
            Relationship = "Parent",
            Phone = "+1-555-0312",
            IsPrimary = true,
        });

        return patient;
    }

    private static void AddHierarchical(
        List<MasterDataItem> items,
        MasterDataType type,
        IReadOnlyList<(string Code, string DisplayName, string ParentCode)> values)
    {
        var sort = 1;
        foreach (var (code, displayName, parentCode) in values)
        {
            items.Add(new MasterDataItem
            {
                Id = CreateDeterministicId(type, code),
                Type = type,
                Code = code,
                DisplayName = displayName,
                ParentCode = parentCode,
                SortOrder = sort++,
                IsActive = true,
            });
        }
    }

    private static void Add(
        List<MasterDataItem> items,
        MasterDataType type,
        IReadOnlyList<(string Code, string DisplayName)> values)
    {
        var sort = 1;
        foreach (var (code, displayName) in values)
        {
            items.Add(new MasterDataItem
            {
                Id = CreateDeterministicId(type, code),
                Type = type,
                Code = code,
                DisplayName = displayName,
                SortOrder = sort++,
                IsActive = true,
            });
        }
    }

    private static Guid CreateDeterministicId(MasterDataType type, string code)
    {
        var key = $"{type}:{code}";
        var bytes = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(key));
        return new Guid(bytes);
    }

    private static void AddContacts(Patient patient, params PatientContact[] contacts)
    {
        foreach (var contact in contacts)
        {
            patient.Contacts.Add(contact);
        }
    }

    private static PatientContact Contact(Guid patientId, ContactType type, string value, bool isPrimary) =>
        new()
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            ContactType = type,
            Value = value,
            IsPrimary = isPrimary,
        };

    private static PatientAddress Address(
        Guid patientId,
        AddressType type,
        string line1,
        string city,
        string state,
        string postalCode,
        string country,
        bool isPrimary) =>
        new()
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            AddressType = type,
            Line1 = line1,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country,
            IsPrimary = isPrimary,
        };
}
