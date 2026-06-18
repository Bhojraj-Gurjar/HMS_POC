# API Documentation

Base URL (development): `https://localhost:7001/api`

All endpoints return JSON wrapped in the standard **`ApiResponse<T>`** envelope unless noted.

## Response Envelope

### Success

```json
{
  "success": true,
  "message": "Optional human-readable message",
  "data": { },
  "errors": null,
  "correlationId": "f47ac10b-58cc-4372-a567-0e02b2c3d479"
}
```

### Failure

```json
{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": ["FirstName is required.", "DateOfBirth is invalid."],
  "correlationId": "f47ac10b-58cc-4372-a567-0e02b2c3d479"
}
```

### HTTP Status Mapping

| Status | When |
|--------|------|
| 200 | Successful GET, PUT, POST (non-create) |
| 201 | Successful POST create |
| 400 | Validation / domain errors |
| 404 | Entity not found |
| 409 | Duplicate patient conflict |
| 500 | Unhandled server error |

### Headers

| Header | Description |
|--------|-------------|
| `X-Correlation-Id` | Optional request correlation ID (echoed in response) |
| `Authorization` | `Bearer <token>` (when auth is enabled) |

---

## Health

### GET `/health`

Returns API health and active data mode.

**Response `data`:**

```json
{
  "status": "Healthy",
  "dataMode": "Mock",
  "configuredDataMode": "SqlServer",
  "mockFallbackActive": true,
  "timestamp": "2026-06-17T10:00:00Z"
}
```

---

## Authentication

### POST `/auth/login`

Placeholder login endpoint for POC demos. **Accepts any username/password** and returns a mock JWT.

**Request:**

```json
{
  "userName": "admin",
  "password": "admin123"
}
```

**Response `data`:**

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 3600,
  "userName": "receptionist",
  "roles": ["Receptionist"]
}
```

---

## Patients (Modern REST)

### GET `/patients`

Search patients with query parameters.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `searchTerm` | string | — | Free-text search (MRN, name, national ID) |
| `pageNumber` | int | 1 | Page index |
| `pageSize` | int | 20 | Page size |

**Response `data`:** `PaginatedList<PatientSummaryDto>`

```json
{
  "items": [
    {
      "id": "11111111-1111-1111-1111-111111111111",
      "patientNumber": "MRN-2026-000001",
      "fullName": "Jane Doe",
      "dateOfBirth": "1990-05-15",
      "gender": "Female",
      "status": "Active",
      "primaryPhone": "+1-555-0100",
      "createdAt": "2026-01-10T08:00:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 5,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

### GET `/patients/{id}`

Get full patient record by GUID.

**Response `data`:** `PatientDto`

### POST `/patients`

Register a new patient.

**Request:** `CreatePatientDto`

```json
{
  "firstName": "Jane",
  "middleName": "Marie",
  "lastName": "Doe",
  "dateOfBirth": "1990-05-15",
  "gender": "Female",
  "bloodGroup": "OPositive",
  "nationalId": "CIV-123456",
  "nationality": "US",
  "allowDuplicateOverride": false,
  "addresses": [
    {
      "addressType": "Home",
      "line1": "123 Main St",
      "city": "Springfield",
      "state": "IL",
      "postalCode": "62701",
      "country": "US",
      "isPrimary": true
    }
  ],
  "contacts": [
    {
      "contactType": "Mobile",
      "value": "+1-555-0100",
      "isPrimary": true
    }
  ],
  "insurances": [],
  "emergencyContacts": []
}
```

**Response:** `201 Created` with `PatientDto` in `data`.

### PUT `/patients/{id}`

Update an existing patient.

**Request:** `UpdatePatientDto` (same shape as create + `status` field)

### DELETE `/patients/{id}`

Soft-delete a patient (sets status to Inactive).

### POST `/patients/duplicate-check`

Check for potential duplicate patients before registration.

**Request:**

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "dateOfBirth": "1990-05-15",
  "nationalId": "CIV-123456",
  "phone": "+1-555-0100"
}
```

**Response `data`:**

```json
{
  "hasDuplicates": true,
  "matches": [
    {
      "id": "11111111-1111-1111-1111-111111111111",
      "patientNumber": "MRN-2026-000001",
      "fullName": "Jane Doe",
      "dateOfBirth": "1990-05-15",
      "gender": "Female",
      "status": "Active",
      "primaryPhone": "+1-555-0100",
      "createdAt": "2026-01-10T08:00:00Z"
    }
  ]
}
```

---

## Patient Registration (Kebab-case REST)

Alternative routes aligned with migration naming conventions.

### POST `/patient-registration`

Create patient — same body as `POST /patients`.

### PUT `/patient-registration/{id}`

Update patient.

### GET `/patient-registration/{id}`

Get patient by ID.

### POST `/patient-registration/search`

Field-level patient search (POST body).

**Request:**

```json
{
  "mrNumber": "MRN-2026-000001",
  "firstName": "Jane",
  "lastName": "Doe",
  "mobileNumber": "+1-555-0100",
  "civilId": "CIV-123456",
  "searchTerm": null,
  "pageNumber": 1,
  "pageSize": 10
}
```

All search fields are optional. Provided fields are combined with AND logic.

**Response `data`:** `PaginatedList<PatientSummaryDto>`

---

## Dropdowns / Master Data

### GET `/dropdowns/{type}`

### GET `/masterdata/{type}`

Retrieve lookup values for a dropdown category.

| Query param | Type | Description |
|-------------|------|-------------|
| `parentCode` | string | Optional. Filters hierarchical types (`State`, `City`, `Area`) by parent code. Example: `GET /api/dropdowns/State?parentCode=US` |

**Cascading usage:** `Country` → `State?parentCode={countryCode}` → `City?parentCode={stateCode}` → `Area?parentCode={cityCode}`

**India example:**

```http
GET /api/masterdata/State?parentCode=IN
GET /api/masterdata/City?parentCode=IN-TN
GET /api/masterdata/Area?parentCode=IN-TN-CHE
```

Mock seed includes **all 36 Indian states/union territories** and **200+ major cities** (see `IndiaGeographySeed.cs`). Other countries use representative sample data.

**Supported types:**

| Type | Description |
|------|-------------|
| `Prefix` | Name prefix (Mr, Mrs, Dr, …) — alias: `PatientTitle` |
| `Gender` | Male, Female, Other |
| `MaritalStatus` | Single, Married, Divorced, … |
| `BloodGroup` | A Positive, O Positive, … |
| `Nationality` | Kuwaiti, American, British, … |
| `Race` | Asian, White, Middle Eastern, … |
| `Religion` | Islam, Christianity, … |
| `Language` | Arabic, English, … |
| `Country` | Kuwait, United States, … |
| `State` | State / province |
| `City` | City |
| `Area` | District / area |
| `Occupation` | Nurse, Engineer, … |
| `Company` | Employer names |
| `Profession` | Medicine, Engineering, … |
| `IncomeCategory` | Low, Middle, High, … |
| `Relationship` | Spouse, Parent, … (emergency contacts) |

**Example:** `GET /api/dropdowns/Gender`

**Response `data`:**

```json
[
  {
    "id": "a1b2c3d4-...",
    "type": "Gender",
    "code": "Female",
    "displayName": "Female",
    "sortOrder": 2,
    "isActive": true
  }
]
```

---

## Legacy Endpoints

Backward-compatible routes for the strangler-fig migration from ASP.NET MVC. All return `ApiResponse<T>`.

### POST `/patientregistration/IUD`

Insert / Update / Delete multiplexer.

**Request:**

```json
{
  "operation": "I",
  "patientID": null,
  "patientData": { },
  "updateData": null
}
```

| Operation | Required fields |
|-----------|-----------------|
| `"I"` (Insert) | `patientData` (`CreatePatientDto`) |
| `"U"` (Update) | `patientID`, `updateData` (`UpdatePatientDto`) |
| `"D"` (Delete) | `patientID` |

**Response `data`:** `LegacyPatientIudResultDto`

```json
{
  "operation": "I",
  "patientID": "11111111-1111-1111-1111-111111111111",
  "patient": { }
}
```

### POST `/patientregistration/Fetch`

Legacy patient search.

**Request:**

```json
{
  "mrNumber": "MRN-2026-000001",
  "firstName": "Jane",
  "lastName": "Doe",
  "mobileNumber": "+1-555-0100",
  "civilID": "CIV-123456",
  "searchTerm": null,
  "pageNumber": 1,
  "pageSize": 20
}
```

**Response `data`:** `PaginatedList<PatientSummaryDto>`

### POST `/patientregistration/FetchPatientData`

Load a single patient by ID or MR Number.

**Request:**

```json
{
  "patientID": "11111111-1111-1111-1111-111111111111",
  "mrNumber": null
}
```

Either `patientID` or `mrNumber` is required.

**Response `data`:** `PatientDto`

### POST `/CommonDropdown/Fetch`

Legacy dropdown fetch.

**Single type:**

```json
{
  "dropdownType": "Gender"
}
```

**Multiple types:**

```json
{
  "dropdownTypes": ["Gender", "BloodGroup", "Nationality"]
}
```

**Response `data`:** `MasterDataItemDto[]` (single type) or `LegacyDropdownBatchResultDto` (multiple types)

```json
{
  "dropdowns": {
    "Gender": [ ],
    "BloodGroup": [ ]
  }
}
```

---

## Data Models

### PatientDto (abbreviated)

| Field | Type | Description |
|-------|------|-------------|
| `id` | guid | Primary key |
| `patientNumber` | string | MR Number (e.g. `MRN-2026-000001`) |
| `firstName` | string | |
| `lastName` | string | |
| `dateOfBirth` | date | ISO `YYYY-MM-DD` |
| `gender` | string | |
| `bloodGroup` | string | |
| `nationalId` | string | Civil ID |
| `nationality` | string | |
| `status` | string | Active, Inactive, Deceased |
| `addresses` | array | |
| `contacts` | array | Mobile, Email, WorkPhone |
| `insurances` | array | |
| `emergencyContacts` | array | |

### PaginatedList\<T\>

| Field | Type |
|-------|------|
| `items` | `T[]` |
| `pageNumber` | int |
| `pageSize` | int |
| `totalCount` | int |
| `totalPages` | int |
| `hasPreviousPage` | bool |
| `hasNextPage` | bool |

---

## Mock Seed Data

When `DataMode` is `Mock`, the following seed data is available immediately:

**Patients:**

| MR Number | Name |
|-----------|------|
| MRN-2026-000001 | Jane Doe |
| MRN-2026-000002 | John Smith |
| MRN-2026-000003 | Aisha Khan |
| MRN-2026-000004 | Robert Chen |
| MRN-2026-000005 | Emily Brown |

**Geography (address cascades):**

| Country | States/UTs | Cities |
|---------|------------|--------|
| India (`IN`) | 36 (all states + union territories) | 200+ major cities per state |
| United States (`US`) | Sample states | Sample cities |
| Kuwait, UAE, UK, Egypt, etc. | Representative data | Representative data |

India data is maintained in `IndiaGeographySeed.cs` and merged into `MockSeedData` at startup.

**Sample search:**

```http
POST /api/patient-registration/search
Content-Type: application/json

{ "firstName": "Jane", "pageNumber": 1, "pageSize": 10 }
```

---

## Swagger

Interactive API documentation is available at:

```
https://localhost:7001/swagger
```

Use Swagger to explore schemas, try requests, and inspect response models during development.

## Frontend Integration

The Angular app uses `ApiBaseService` to unwrap `ApiResponse<T>`:

```typescript
// environment.ts
apiUrl: '/api'

// proxy.conf.json (dev)
"/api" → "https://localhost:7001"
```

Set `useMockPatientSearch: false` in `environment.ts` to use the live API for patient search (recommended when backend is running). The dropdown service calls `refresh()` on parent changes to avoid stale cascade cache after seed updates.
