# Architecture

This document describes the system architecture of the HMS Patient Registration Migration POC.

## Overview

The system follows a **client–server** model with a single-page Angular application communicating with a .NET 8 REST API. The backend uses **Clean Architecture** with clear separation between domain rules, application orchestration, infrastructure, and HTTP delivery.

```
┌─────────────────────────────────────────────────────────────────┐
│                     Angular 18 SPA (Browser)                    │
│  core │ shared │ layout │ features (auth, dashboard, patients)  │
└────────────────────────────┬────────────────────────────────────┘
                             │ HTTPS /api/*
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│              HMS.PatientRegistration.Api                          │
│  Controllers → Application Services → MediatR → Repositories      │
│  Middleware: CorrelationId, ExceptionHandling, CORS, JWT          │
└────────────────────────────┬────────────────────────────────────┘
                             │
              ┌──────────────┴──────────────┐
              ▼                             ▼
   ┌────────────────────┐       ┌────────────────────┐
   │   Mock Repositories │       │  EF Core + SQL Server│
   │   (in-memory)       │       │  (PatientRepository) │
   └────────────────────┘       └────────────────────┘
```

## Tech Stack

### Frontend

| Concern | Choice |
|---------|--------|
| Framework | Angular 18 (standalone components, lazy routes) |
| UI | Angular Material 18 |
| Forms | Reactive Forms (strongly typed `FormGroup` slices) |
| State | Angular signals (feature stores), RxJS for async |
| HTTP | `HttpClient` + interceptors (auth, error, loading) |
| Styling | SCSS, Material theming |
| Tooling | ESLint, Prettier, TypeScript strict mode |

### Backend

| Concern | Choice |
|---------|--------|
| Runtime | .NET 8 |
| API | ASP.NET Core Web API |
| Pattern | Clean Architecture + CQRS (MediatR) |
| Validation | FluentValidation pipeline behavior |
| Mapping | AutoMapper |
| ORM | Entity Framework Core 8 |
| API docs | Swagger / OpenAPI |
| Auth | JWT Bearer (placeholder; `[AllowAnonymous]` in POC) |

## Folder Structure

### Repository root

```
HMS/
├── docs/                    # Architecture, API, migration docs
├── README.md
└── src/
    ├── backend/
    └── frontend/
```

### Backend (`src/backend/`)

```
src/backend/
├── HMS.PatientRegistration.sln
├── tests/HMS.PatientRegistration.Tests/
└── src/
    ├── HMS.PatientRegistration.Domain/
    │   ├── Entities/              Patient, PatientAddress, MasterDataItem, …
    │   ├── Enums/                 Gender, BloodGroup, MasterDataType, …
    │   └── Exceptions/            DomainException, NotFoundException, …
    │
    ├── HMS.PatientRegistration.Application/
    │   ├── Common/
    │   │   ├── Behaviors/         ValidationBehavior, LoggingBehavior
    │   │   ├── Helpers/           EnumParser, PatientSearchCriteriaMapper
    │   │   ├── Interfaces/        IUnitOfWork, repository + service contracts
    │   │   ├── Mappings/          AutoMapper profiles
    │   │   ├── Models/            ApiResponse, PaginatedList, PatientSearchCriteria
    │   │   └── Settings/          DataModeSettings
    │   ├── Patients/
    │   │   ├── Commands/          Create, Update, Delete
    │   │   ├── Queries/           GetById, Search, CheckDuplicate
    │   │   └── DTOs/
    │   ├── MasterData/Queries/
    │   ├── Legacy/DTOs/           Legacy adapter request/response shapes
    │   ├── Services/              PatientRegistrationService, DropdownService, …
    │   └── DependencyInjection.cs
    │
    ├── HMS.PatientRegistration.Infrastructure/
    │   ├── Persistence/
    │   │   ├── ApplicationDbContext.cs
    │   │   ├── Configurations/    EF entity configurations
    │   │   ├── Repositories/      Patient, MasterData, Mock variants
    │   │   └── Seed/              MockSeedData, IndiaGeographySeed, DatabaseSeeder
    │   ├── Services/              CurrentUserService, DateTimeProvider
    │   └── DependencyInjection.cs
    │
    └── HMS.PatientRegistration.Api/
        ├── Controllers/           REST + Legacy adapters
        ├── Middleware/            CorrelationId, ExceptionHandling, RequestLogging
        ├── Extensions/            Swagger, CORS, JWT registration
        ├── Program.cs
        └── appsettings.json
```

### Frontend (`src/frontend/hms-patient-registration/`)

```
src/app/
├── core/
│   ├── auth/                  Guards (auth, guest)
│   ├── interceptors/          Auth, correlation ID, error
│   ├── models/                ApiResponse, patient, auth types
│   └── services/              PatientService, MasterDataService, AuthService, …
├── shared/ui/                 Loading overlay
├── layout/main-layout/        Sidebar + header shell
└── features/
    ├── auth/                  Login, unauthorized
    ├── dashboard/             KPIs, charts, recent patients
    ├── patients/              List, registration, detail, edit, search dialog
    ├── appointments/          Placeholder
    └── reports/               Placeholder

src/styles/_page-layout.scss   # Shared hms-* page layout
```

## Backend Layers

### Domain

Pure business entities and enums. No framework dependencies. Domain exceptions express business rule violations (`DuplicatePatientException`, `NotFoundException`).

### Application

- **Commands / Queries** — MediatR handlers contain business logic
- **DTOs** — API contracts (`CreatePatientDto`, `PatientSearchRequestDto`, …)
- **Application services** — thin facades used by controllers (`IPatientRegistrationService`)
- **Validators** — FluentValidation for command/query inputs
- **Behaviors** — cross-cutting validation and logging pipelines

### Infrastructure

- **Repositories** — data access (`IPatientRepository`, `IMasterDataRepository`)
- **Mock repositories** — thread-safe in-memory stores for POC/demo
- **EF Core** — `ApplicationDbContext`, migrations, SQL Server provider
- **Seed data** — `MockSeedData` shared between Mock and SQL Server first-run seed

### API

Thin controllers. No business logic. Responsibilities:

1. Accept HTTP request
2. Delegate to application service or MediatR
3. Wrap result in `ApiResponse<T>`
4. Return appropriate HTTP status

## Frontend Layers

### Core

Singleton services, guards, interceptors, and shared interfaces. Feature modules depend on core; core never depends on features.

### Shared

Reusable presentational components, pipes, and validators with no feature-specific knowledge.

### Features

Lazy-loaded functional areas:

- **Dashboard** — hero header, clickable KPI cards, charts, recent patients (`PatientService.getDashboardStats()`)
- **Patient list** — hero header, search/filter table, colored status chips
- **Patient registration** — single `PatientRegistrationComponent` with reactive form, search dialog, document upload, recent-patients sidebar
- **Appointments / Reports** — placeholder screens with shared hero layout

### UI Layout System

Primary screens use `src/styles/_page-layout.scss`:

| Class | Purpose |
|-------|---------|
| `hms-page` | Page container (max-width 1400px, vertical gap) |
| `hms-hero` | Blue gradient header with eyebrow, title, subtitle, actions |
| `hms-panels` | Two-column main + sidebar layout |
| `hms-panel` | Material card panel styling |
| `hms-quick-actions` | Sidebar action list |

### Address Cascading Dropdowns

```
Country (e.g. IN)
  → GET /masterdata/State?parentCode=IN
    → GET /masterdata/City?parentCode=IN-TN
      → GET /masterdata/Area?parentCode=IN-TN-CHE
```

`MasterDataService` loads items per level; the registration form resets child dropdowns when a parent changes.

### Data Flow (Registration)

```
Search dialog → PatientService.getById() → patch form → edit mode
Save → duplicate check (create) → PatientService.create() / update()
Documents → stored in notes JSON as registrationMeta.documents (base64 POC)
```

## Data Modes

Controlled by `DataMode.Mode` in `appsettings.json`.

### Mock Mode

```json
"DataMode": { "Mode": "Mock" }
```

- Registers `MockPatientRepository` and `MockMasterDataRepository`
- Uses `MockUnitOfWork` (no-op save)
- Seeds **5 patients** and **17 dropdown types** from `MockSeedData` + `IndiaGeographySeed`
- Ideal for local development, demos, and CI without SQL Server

### SQL Server Mode

```json
"DataMode": { "Mode": "SqlServer" },
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HMS_PatientRegistration;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

- Registers EF Core `ApplicationDbContext`, real repositories, `UnitOfWork`
- `DatabaseInitializerHostedService` runs migrations and `DatabaseSeeder` on startup
- Same seed data as Mock mode when tables are empty

### Switching modes

1. Change `DataMode.Mode` in `appsettings.json` (or `appsettings.Development.json`)
2. Restart the API
3. Confirm via `GET /api/health`

For SQL Server first-time setup:

```bash
cd src/backend
dotnet ef migrations add InitialCreate \
  --project src/HMS.PatientRegistration.Infrastructure \
  --startup-project src/HMS.PatientRegistration.Api

dotnet ef database update \
  --project src/HMS.PatientRegistration.Infrastructure \
  --startup-project src/HMS.PatientRegistration.Api
```

## API Response Envelope

All API responses use a consistent wrapper:

```json
{
  "success": true,
  "message": "Optional message",
  "data": { },
  "errors": null,
  "correlationId": "abc-123"
}
```

Errors are handled by `ExceptionHandlingMiddleware` and mapped to HTTP status codes (400, 404, 409, 500).

## Legacy Adapter Pattern

Legacy MVC-style endpoints are exposed under `Controllers/Legacy/`:

| Legacy route | Modern equivalent |
|--------------|-------------------|
| `POST /api/patientregistration/IUD` | Create / Update / Delete commands |
| `POST /api/patientregistration/Fetch` | Search query |
| `POST /api/patientregistration/FetchPatientData` | GetById query |
| `POST /api/CommonDropdown/Fetch` | Master data query |

Adapters translate legacy DTOs and delegate to the same application services — no duplicated business logic.

## Cross-Cutting Concerns

| Concern | Implementation |
|---------|----------------|
| Correlation ID | `X-Correlation-Id` header via middleware |
| Logging | `LoggingBehavior` (MediatR), `RequestLoggingMiddleware` (HTTP), ASP.NET Core logging |
| CORS | `DefaultCors` policy for `localhost:4200` |
| Validation | FluentValidation + reactive form validators (frontend) |
| Error display | `NotificationService` + Material snackbars (frontend) |

## Security (POC State)

- JWT middleware is configured but controllers use `[AllowAnonymous]`
- Secret key in `appsettings.json` is a placeholder — replace for any non-local use
- Frontend stores token in `localStorage` via `STORAGE_KEYS`

Production migration should enable `[Authorize]`, role policies, HTTPS-only cookies or secure token storage, and secret management (Azure Key Vault, etc.).

## Deployment Topology (Target)

```
[Browser] → [CDN / Static host — Angular build]
         → [API Gateway / Load Balancer]
         → [.NET 8 API instances]
         → [SQL Server]
```

The POC runs all components locally with the Angular dev proxy bridging frontend to backend.
