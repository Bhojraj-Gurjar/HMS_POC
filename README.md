# HMS Patient Registration — Migration POC

Hospital Management System (HMS) proof-of-concept for migrating **Patient Registration** from a legacy **AngularJS + ASP.NET MVC** stack to **Angular 18 + .NET 8 Web API**.

This repository contains a full-stack reference implementation: modular frontend, Clean Architecture backend, mock and SQL Server data modes, legacy API adapters, and a hospital-grade registration UI.

## Documentation

| Document | Description |
|----------|-------------|
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | System design, folder structure, layers, and data flow |
| [docs/API_DOCUMENTATION.md](docs/API_DOCUMENTATION.md) | REST endpoints, request/response contracts, `ApiResponse` envelope |
| [docs/MIGRATION_STRATEGY.md](docs/MIGRATION_STRATEGY.md) | Phased migration plan from AngularJS + MVC to Angular + .NET 8 |

## Tech Stack

| Layer | Technologies |
|-------|--------------|
| **Frontend** | Angular 18 (standalone), Angular Material, RxJS, Reactive Forms, TypeScript 5.5 |
| **Backend** | .NET 8, ASP.NET Core Web API, MediatR (CQRS), FluentValidation, AutoMapper |
| **Data** | EF Core 8, SQL Server, in-memory Mock repositories |
| **Cross-cutting** | JWT (placeholder), Swagger/OpenAPI, CORS, correlation IDs, global exception handling |

## Repository Structure

```
HMS/
├── docs/                          # Project documentation
│   ├── ARCHITECTURE.md
│   ├── API_DOCUMENTATION.md
│   └── MIGRATION_STRATEGY.md
├── src/
│   ├── backend/                   # .NET 8 Clean Architecture solution
│   │   ├── HMS.PatientRegistration.sln
│   │   ├── tests/HMS.PatientRegistration.Tests/
│   │   └── src/
│   │       ├── HMS.PatientRegistration.Api/
│   │       ├── HMS.PatientRegistration.Application/
│   │       ├── HMS.PatientRegistration.Domain/
│   │       └── HMS.PatientRegistration.Infrastructure/
│   └── frontend/
│       └── hms-patient-registration/   # Angular 18 SPA
└── README.md
```

## Prerequisites

| Tool | Version | Required for |
|------|---------|--------------|
| [Node.js](https://nodejs.org/) | 20+ | Frontend |
| [npm](https://www.npmjs.com/) | 10+ | Frontend |
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | 8.0 | Backend |
| [SQL Server](https://www.microsoft.com/sql-server) | 2019+ | SQL Server mode only |

## Quick Start

### 1. Clone and open

```bash
git clone <repository-url>
cd HMS
```

### 2. Run the backend (Mock mode — default)

```bash
cd src/backend
dotnet restore
dotnet build
dotnet run --project src/HMS.PatientRegistration.Api
```

- API: `https://localhost:7001`
- Swagger: `https://localhost:7001/swagger` (or `http://localhost:5001/swagger` if HTTPS cert issues)
- Health check: `GET https://localhost:7001/api/health`

**HTTPS certificate warning?** Run once (Windows):

```bash
dotnet dev-certs https --trust
```

Then restart your browser and open Swagger again.

### 3. Run the frontend

In a second terminal:

```bash
cd src/frontend/hms-patient-registration
npm install
npm start
```

- App: `http://localhost:4200`
- Dev proxy forwards `/api` → `https://localhost:7001`

### 4. Log in

Use the login screen (mock JWT — accepts any credentials for the POC). Example demo login:

| Username | Password |
|----------|----------|
| `admin` | `admin123` |

Navigate to **Dashboard**, **Patients**, or **Patients → Register** to exercise the registration workflow.

## Data Modes

Configure in `src/backend/src/HMS.PatientRegistration.Api/appsettings.json`:

```json
"DataMode": {
  "Mode": "Mock"
}
```

| Mode | Value | Description |
|------|-------|-------------|
| **Mock** | `"Mock"` | In-memory repositories; 5 seeded patients + full dropdown catalog (incl. all India states/UTs and 200+ cities). No database required. |
| **SQL Server** | `"SqlServer"` or `"Database"` | EF Core + SQL Server; auto-migrates and seeds on first run. |
| **Auto fallback** | `"AllowMockFallback": true` | If SQL Server mode is configured but the database is unreachable at startup, the API automatically uses mock repositories. |

Verify active mode:

```http
GET /api/health
```

Response includes `"dataMode"` (effective mode), `"configuredDataMode"`, and `"mockFallbackActive"`.

See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md#data-modes) for switching to SQL Server.

## Manager Requirements Compliance

This POC is aligned to the **HMS Patient Registration Migration** specification. Summary:

| Area | Status | Notes |
|------|--------|-------|
| Angular 18 + Material + Reactive Forms | Done | Standalone components, lazy routes |
| Patient Type (New / Existing / Staff / Newborn) | Done | Toggle selector on registration screen |
| Personal details (all manager fields) | Done | Prefix API, age Y/M/D, mobile country + 10-digit number; last name (no duplicate family name field) |
| Additional details collapsible | Done | Nationality, warnings, race, religion, language |
| Address 4-level cascade | Done | Country → State → City → Area + pincode; phone captured in personal details only |
| Dashboard-style UI | Done | Shared `hms-*` layout on dashboard, patient list, and registration |
| Professional dropdowns | Done | Occupation, company, profession, income category |
| Insurance & document placeholders | Done | Expansion panels |
| Appointment reference field | Done | Integration-ready input |
| Patient search modal | Done | MR, name, mobile, civil ID |
| Custom validators | Done | Email, 10-digit mobile, name, DOB, pincode, civil ID |
| .NET 8 Clean Architecture | Done | Api / Application / Domain / Infrastructure |
| Legacy + modern API endpoints | Done | `/api/patientregistration/*`, `/api/patient-registration/*` |
| Mock + SQL Server modes | Done | `AllowMockFallback` auto-fallback |
| Stored procedure placeholders | Done | `IStoredProcedurePatientRepository` |
| Logging middleware | Done | `RequestLoggingMiddleware` |
| Tests project | Done | `tests/HMS.PatientRegistration.Tests` |
| Swagger + ApiResponse wrapper | Done | |
| Documentation (README + /docs) | Done | |

**Intentional naming differences (no functional gap):** Domain entity `Patient` (not `PatientRegistration`), `MasterDataItem` (not `DropdownItem`), feature path `features/patients/patient-registration` (extra `patients` parent for list/detail/edit routes).

## POC Requirements Checklist

| # | Requirement | Status |
|---|-------------|--------|
| 1 | Working Angular Patient Registration UI | Done — `/patients/register` |
| 2 | Working .NET 8 Web API backend structure | Done — Clean Architecture solution |
| 3 | Mock data fallback if SQL Server is not connected | Done — `AllowMockFallback` + health endpoint |
| 4 | Swagger documentation | Done — `https://localhost:7001/swagger` |
| 5 | API response wrapper | Done — `ApiResponse<T>` envelope |
| 6 | Patient search modal | Done — search dialog with multi-field filters |
| 7 | Cascading dropdowns | Done — Country → State → City → Area (full India geography in mock seed) |
| 8 | Save/update patient flow | Done — create, duplicate check, update |
| 9 | Form reset flow | Done — New / Reset on action bar |
| 10 | Age calculation from DOB | Done — auto-calculated readonly age field |
| 11 | Proper validation messages | Done — `app-validation-message` on required fields |
| 12 | README.md with setup steps | Done — this file |
| 13 | Migration strategy document | Done — [docs/MIGRATION_STRATEGY.md](docs/MIGRATION_STRATEGY.md) |
| 14 | Architecture document | Done — [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) |
| 15 | API documentation | Done — [docs/API_DOCUMENTATION.md](docs/API_DOCUMENTATION.md) |

## Frontend Routes

| Route | Feature |
|-------|---------|
| `/login` | Authentication |
| `/dashboard` | Overview — KPI cards, recent patients, quick actions |
| `/patients` | Patient directory — dashboard-style search and table |
| `/patients/register` | Patient registration — hero, setup strip, form sections, quick actions sidebar |
| `/patients/:id` | Patient detail |
| `/patients/:id/edit` | Patient edit |

## Key Features (POC)

- **Unified hospital UI** — dashboard, patient list, and registration share the same `hms-*` page layout (hero banner, panels, quick actions)
- **Patient registration** — single-column form sections with sticky action bar (New / Search / Save / Reset)
- **Patient search dialog** — MR Number, name, mobile, civil ID search
- **Duplicate detection** — pre-save duplicate check with override dialog
- **Dropdown service** — cached master data with cascading Country → State → City → Area; India includes all 36 states/UTs and major cities per state
- **Legacy API adapters** — backward-compatible endpoints for strangler-fig migration
- **Mock mode** — full demo without SQL Server

## Scripts Reference

### Backend

```bash
cd src/backend
dotnet build
dotnet run --project src/HMS.PatientRegistration.Api
dotnet test   # HMS.PatientRegistration.Tests
```

### Frontend

```bash
cd src/frontend/hms-patient-registration
npm start          # dev server + proxy
npm run build      # production build
npm run lint       # ESLint
npm run format     # Prettier
```

## Environment Configuration

| File | Purpose |
|------|---------|
| `src/backend/.../appsettings.json` | DataMode, connection strings, JWT, CORS |
| `src/frontend/.../environment.ts` | `apiUrl`, `useMockPatientSearch` |
| `src/frontend/.../proxy.conf.json` | Dev API proxy target |

## Future Enhancements

See [docs/MIGRATION_STRATEGY.md](docs/MIGRATION_STRATEGY.md#future-enhancements) for the full roadmap. Highlights:

- Real JWT authentication and role-based authorization
- Remaining HMS modules (appointments, billing, admissions)
- E2E and integration test suites
- CI/CD pipelines and containerization
- Production hardening (logging, monitoring, rate limiting)
- Full legacy API deprecation and removal

## License

Internal / POC — adjust per organizational policy.
