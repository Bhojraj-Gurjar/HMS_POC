# HMS Patient Registration — Backend

.NET 8 Clean Architecture Web API for the HMS Patient Registration Migration POC.

## Projects

| Project | Purpose |
|---------|---------|
| `HMS.PatientRegistration.Domain` | Entities, enums, domain exceptions |
| `HMS.PatientRegistration.Application` | CQRS, DTOs, validators, application services |
| `HMS.PatientRegistration.Infrastructure` | EF Core, repositories, mock mode, seed data |
| `HMS.PatientRegistration.Api` | HTTP API, middleware, Swagger |
| `tests/HMS.PatientRegistration.Tests` | Unit tests |

## Run

```bash
cd src/backend
dotnet restore
dotnet build
dotnet run --project src/HMS.PatientRegistration.Api
```

| URL | Purpose |
|-----|---------|
| `https://localhost:7001/swagger` | Swagger UI (Development) |
| `GET /api/health` | Health + data mode |

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
| `Mock` | In-memory repositories (default) |
| `SqlServer` / `Database` | EF Core + SQL Server |
| `AllowMockFallback` | Falls back to mock if SQL Server is unreachable |

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/health` | Health + data mode |
| POST | `/api/auth/login` | Login placeholder |
| GET | `/api/dashboard/stats` | Dashboard KPIs and charts |
| GET | `/api/patients` | Search patients (`searchTerm`, `pageNumber`, `pageSize`) |
| GET | `/api/patients/{id}` | Get patient |
| POST | `/api/patients` | Register patient |
| PUT | `/api/patients/{id}` | Update patient |
| DELETE | `/api/patients/{id}` | Soft delete |
| POST | `/api/patients/duplicate-check` | Duplicate detection |
| POST | `/api/patient-registration` | Create (kebab-case alias) |
| PUT | `/api/patient-registration/{id}` | Update |
| GET | `/api/patient-registration/{id}` | Get by ID |
| POST | `/api/patient-registration/search` | Field-level search |
| GET | `/api/masterdata/{type}` | Lookup values (`?parentCode=` for cascade) |
| GET | `/api/dropdowns/{type}` | Alias of masterdata |
| POST | `/api/patientregistration/IUD` | Legacy insert/update/delete |
| POST | `/api/patientregistration/Fetch` | Legacy search |
| POST | `/api/patientregistration/FetchPatientData` | Legacy get by ID/MRN |
| POST | `/api/CommonDropdown/Fetch` | Legacy dropdown fetch |

Full contracts: [docs/API_DOCUMENTATION.md](../../docs/API_DOCUMENTATION.md)

## Swagger

Swagger is enabled in **Development** with grouped tags, XML summaries, and Try it out:

```
https://localhost:7001/swagger
```

## Tests

```bash
cd src/backend
dotnet test
```

## SQL Server Setup

```json
"DataMode": { "Mode": "SqlServer" },
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HMS_PatientRegistration;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

```bash
dotnet ef migrations add InitialCreate \
  --project src/HMS.PatientRegistration.Infrastructure \
  --startup-project src/HMS.PatientRegistration.Api

dotnet ef database update \
  --project src/HMS.PatientRegistration.Infrastructure \
  --startup-project src/HMS.PatientRegistration.Api
```
