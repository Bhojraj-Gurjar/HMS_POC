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
src/
├── app/
│   ├── core/
│   │   ├── auth/                  Guards, auth service
│   │   ├── constants/             API endpoints, storage keys
│   │   ├── factories/             Empty model factories
│   │   ├── interceptors/          Auth, error, loading
│   │   ├── interfaces/            Domain TypeScript interfaces
│   │   ├── mappers/               DTO ↔ domain mappers
│   │   └── services/              API, dropdown, notification, validation
│   │
│   ├── shared/
│   │   ├── pipes/                 AppDatePipe, …
│   │   ├── ui/                    PageHeader, EmptyState, ValidationMessage
│   │   └── validators/            DOB, phone, …
│   │
│   ├── layout/
│   │   ├── main-layout/
│   │   ├── header/
│   │   └── side-nav/
│   │
│   ├── features/
│   │   ├── auth/                  Login, unauthorized
│   │   ├── dashboard/
│   │   └── patients/
│   │       ├── patient-registration/     Main registration feature
│   │       │   ├── components/           Personal, address, search dialog, …
│   │       │   ├── services/               Form service
│   │       │   └── models/               Typed form models
│   │       ├── patient-list/
│   │       ├── patient-detail/
│   │       ├── patient-edit/
│   │       ├── patient-registration-wizard/  Legacy wizard (not routed)
│   │       ├── services/                   PatientApiService
│   │       └── state/                      Signal-based store
│   │
│   ├── app.routes.ts
│   └── app.config.ts
│
├── environments/
├── styles/
│   └── _page-layout.scss        # Shared hms-* dashboard-style page layout
└── main.ts
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

Lazy-loaded functional areas. Patient registration is the primary POC feature:

- **Shell** — `PatientRegistrationComponent` with hero banner, setup strip, quick-actions sidebar, and sticky action bar
- **Child components** — personal details, address, professional, additional, documents
- **Form service** — `PatientRegistrationFormService` builds and patches typed `FormGroup`
- **Search dialog** — reusable `PatientSearchDialogService` + Material dialog
- **Patient list** — dashboard-style directory with hero, KPI row, search panel, and data table
- **Dashboard** — welcome hero, stat cards, recent registrations, quick actions

### UI Layout System

All primary screens (dashboard, patient list, registration) use a shared layout defined in `src/styles/_page-layout.scss`:

| Class prefix | Purpose |
|--------------|---------|
| `hms-page` | Page container (max-width 1400px, vertical gap) |
| `hms-hero` | Blue gradient header with title and actions |
| `hms-stats-row` | KPI stat cards (dashboard and patient list) |
| `hms-panels` | Two-column main + sidebar layout |
| `hms-panel` | Material card panel styling |
| `hms-quick-actions` | Sidebar action list |

Registration uses a compact **setup strip** (patient type + appointment reference) instead of summary stat cards. Form fields are **single-column** within each section card.

### Address Cascading Dropdowns

The address form loads hierarchical master data via `DropdownService`:

```
Country (e.g. IN)
  → State?parentCode=IN     (36 states/UTs for India)
    → City?parentCode=IN-TN (major cities per state)
      → Area?parentCode=IN-TN-CHE (sample areas for select cities)
```

`PatientAddressComponent` uses `control.enable()` / `disable()` (not `[disabled]` on `mat-select`) and tracks per-index loading state. Parent changes call `dropdownService.refresh()` to avoid stale cached empty results.

India geography is seeded from `IndiaGeographySeed.cs` (all states/UTs + 200+ cities). Other countries retain representative sample data in `MockSeedData.cs`.

### Data Flow (Registration)

```
Action Bar (Search)
    → PatientSearchDialogService.open()
    → PatientSearchService (mock or API)
    → patchFromRegistration() on form
    → isEditMode = true

Action Bar (Save)
    → Form validation
    → Duplicate check (create only)
    → PatientApiService.create() / update()
    → NotificationService feedback
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

Legacy MVC-style endpoints are preserved as adapter controllers under `Controllers/Legacy/`:

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
| Error display | `ErrorHandlerService` + Material snackbars (frontend) |

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
