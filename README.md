# HMS Patient Registration — Migration POC

Hospital Management System proof-of-concept migrating **Patient Registration** from **AngularJS + ASP.NET MVC** to **Angular 18 + .NET 8 Web API**.

## Documentation

| Document | Description |
|----------|-------------|
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | System design, layers, and data flow |
| [docs/API_DOCUMENTATION.md](docs/API_DOCUMENTATION.md) | REST endpoints and `ApiResponse` contracts |
| [docs/MIGRATION_STRATEGY.md](docs/MIGRATION_STRATEGY.md) | Phased migration plan |

## Tech Stack

| Layer | Technologies |
|-------|--------------|
| **Frontend** | Angular 18, Angular Material, RxJS, Reactive Forms |
| **Backend** | .NET 8, ASP.NET Core Web API, MediatR, FluentValidation, EF Core 8 |
| **Data** | Mock (in-memory) or SQL Server |
| **API docs** | Swagger / OpenAPI at `/swagger` (Development) |

## Quick Start

### Backend

```bash
cd src/backend
dotnet restore
dotnet build
dotnet run --project src/HMS.PatientRegistration.Api
```

| URL | Purpose |
|-----|---------|
| `https://localhost:7001` | API (HTTPS) |
| `http://localhost:5001` | API (HTTP) |
| `https://localhost:7001/swagger` | **Swagger UI** |
| `GET /api/health` | Health + data mode |

Trust dev HTTPS cert (once): `dotnet dev-certs https --trust`

### Frontend

```bash
cd src/frontend/hms-patient-registration
npm install
npm start
```

App: `http://localhost:4200` — dev proxy forwards `/api` → `http://localhost:5001`

### Login

| Username | Password |
|----------|----------|
| `admin` | `admin123` |

## Data Modes

In `src/backend/src/HMS.PatientRegistration.Api/appsettings.json`:

```json
"DataMode": {
  "Mode": "Mock",
  "AllowMockFallback": true
}
```

| Mode | Value | Description |
|------|-------|-------------|
| Mock | `"Mock"` | In-memory data; 5 seeded patients + full dropdown catalog (default) |
| SQL Server | `"SqlServer"` | EF Core + auto-migrate/seed |
| Fallback | `"AllowMockFallback": true` | Uses mock if SQL Server is unreachable |

## Features

- **Dashboard** — KPI cards (clickable), charts, recent patients, unified blue hero header
- **Patient registration** — New / Existing / Staff / Newborn workflows, address cascade, document upload (stored in patient notes JSON)
- **Patient list** — Search, status filters, colored status chips
- **Master data** — Cascading Country → State → City → Area
- **Legacy adapters** — `POST /api/patientregistration/*`, `POST /api/CommonDropdown/Fetch`
- **Mock + SQL Server** — Dual data mode with health endpoint reporting active mode

## Frontend Routes

| Route | Feature |
|-------|---------|
| `/login` | Authentication |
| `/dashboard` | Overview |
| `/patients` | Patient directory |
| `/patients/register` | Registration / update |
| `/patients/:id` | Patient detail |
| `/appointments` | Appointments placeholder |
| `/reports` | Reports placeholder |

## Swagger

Interactive API documentation (all endpoints, schemas, Try it out):

```
https://localhost:7001/swagger
```

Endpoint groups: **Health**, **Authentication**, **Dashboard**, **Patients**, **Patient Registration**, **Master Data**, **Legacy Adapters**.

See [docs/API_DOCUMENTATION.md](docs/API_DOCUMENTATION.md) for request/response examples.

## Scripts

```bash
# Backend
cd src/backend && dotnet test

# Frontend
cd src/frontend/hms-patient-registration
npm run build
npm run lint
```

## License

Internal / POC — adjust per organizational policy.
