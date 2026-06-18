# HMS Patient Registration — Backend

.NET 8 Clean Architecture Web API for the HMS Patient Registration Migration POC.

## Projects

| Project | Purpose |
|---------|---------|
| `HMS.PatientRegistration.Domain` | Entities, enums, domain exceptions |
| `HMS.PatientRegistration.Application` | CQRS (MediatR), DTOs, validators, mappings |
| `HMS.PatientRegistration.Infrastructure` | EF Core, repositories, mock mode, seed data, DI |
| `HMS.PatientRegistration.Api` | HTTP API, middleware, Swagger |
| `tests/HMS.PatientRegistration.Tests` | Unit tests |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (only when `DataMode` is `Database`)

## Configuration

`appsettings.json`:

```json
"DataMode": {
  "Mode": "Mock",
  "AllowMockFallback": true
}
```

| Mode | Description |
|------|-------------|
| `Mock` | In-memory repositories with 5 seeded patients and full dropdown catalog (default) |
| `SqlServer` or `Database` | EF Core + SQL Server; migrates and seeds the same data on first run |
| `AllowMockFallback` | Falls back to mock repositories if SQL Server is unreachable at startup |

Verify active mode: `GET /api/health` returns `dataMode`, `configuredDataMode`, and `mockFallbackActive`.

### Switch to SQL Server

```json
"DataMode": {
  "Mode": "SqlServer"
},
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HMS_PatientRegistration;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## Run

```bash
cd src/backend
dotnet restore
dotnet build
dotnet run --project src/HMS.PatientRegistration.Api
```

Swagger: `https://localhost:7001/swagger` (or `http://localhost:5001/swagger`)

## Tests

```bash
cd src/backend
dotnet test
```

## Database Mode

1. Set `"DataMode": { "Mode": "SqlServer" }`
2. Update `ConnectionStrings:DefaultConnection`
3. Create migration:

```bash
dotnet ef migrations add InitialCreate \
  --project src/HMS.PatientRegistration.Infrastructure \
  --startup-project src/HMS.PatientRegistration.Api

dotnet ef database update \
  --project src/HMS.PatientRegistration.Infrastructure \
  --startup-project src/HMS.PatientRegistration.Api
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/health` | Health + data mode |
| POST | `/api/auth/login` | Login placeholder (any credentials) |
| GET | `/api/patients` | Search patients |
| GET | `/api/patients/{id}` | Get patient |
| POST | `/api/patients` | Register patient |
| PUT | `/api/patients/{id}` | Update patient |
| DELETE | `/api/patients/{id}` | Soft delete |
| POST | `/api/patients/duplicate-check` | Duplicate detection |
| GET | `/api/dropdowns/{type}` | Lookup values (alias of `/api/masterdata/{type}`) |
| GET | `/api/masterdata/{type}` | Lookup values; use `?parentCode=` for State/City/Area |

### Cascading address example (India)

```http
GET /api/masterdata/State?parentCode=IN
GET /api/masterdata/City?parentCode=IN-TN
GET /api/masterdata/Area?parentCode=IN-TN-CHE
```

### Mock seed data

When `DataMode` is `Mock` (or on first SQL Server run), the API includes:

**Patients (5):** Jane Doe, John Smith, Aisha Khan, Robert Chen, Emily Brown (`MRN-2026-000001` … `000005`).

**Dropdown types:** `Prefix`, `Gender`, `MaritalStatus`, `BloodGroup`, `Nationality`, `Race`, `Religion`, `Language`, `Country`, `State`, `City`, `Area`, `Occupation`, `Company`, `Profession`, `IncomeCategory`, `Relationship`.

**India geography** (`IndiaGeographySeed.cs`):

- 36 states and union territories under country code `IN`
- 200+ major cities linked to state codes (e.g. `IN-TN` → Chennai, Coimbatore, …)

Other countries use representative sample states/cities in `MockSeedData.cs`.

`PatientTitle` is accepted as an alias for `Prefix`.

### Middleware

| Middleware | Purpose |
|------------|---------|
| `CorrelationIdMiddleware` | Request correlation ID |
| `ExceptionHandlingMiddleware` | Global error → `ApiResponse` |
| `RequestLoggingMiddleware` | HTTP request/response logging |

All responses use the `ApiResponse<T>` wrapper.
