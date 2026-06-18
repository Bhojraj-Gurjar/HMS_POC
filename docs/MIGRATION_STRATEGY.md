# Migration Strategy

This document outlines the strategy for migrating the HMS Patient Registration module from **AngularJS + ASP.NET MVC** to **Angular 18 + .NET 8 Web API**.

## Executive Summary

The legacy stack couples UI rendering, business logic, and data access in MVC controllers and AngularJS services. The target stack separates concerns into a SPA frontend and a Clean Architecture API, connected via REST. A **strangler-fig pattern** allows incremental migration: legacy endpoints remain available while new consumers adopt modern routes.

```
┌──────────────────────────────────────────────────────────────┐
│                    LEGACY (Current State)                     │
│  AngularJS 1.x  ←→  ASP.NET MVC Controllers  ←→  SQL Server  │
│  (directives, $http)   (IUD/Fetch actions)                    │
└──────────────────────────────────────────────────────────────┘
                              │
                    Strangler-fig migration
                              ▼
┌──────────────────────────────────────────────────────────────┐
│                    TARGET (This POC)                          │
│  Angular 18 SPA  ←→  .NET 8 Web API  ←→  SQL Server / Mock   │
│  (standalone, Material)  (REST + legacy adapters)             │
└──────────────────────────────────────────────────────────────┘
```

## Legacy vs Target Comparison

| Aspect | Legacy | Target |
|--------|--------|--------|
| Frontend | AngularJS 1.x, directives, `$scope` | Angular 18 standalone, signals, Material |
| Backend | ASP.NET MVC, controller actions | ASP.NET Core Web API, Clean Architecture |
| API style | `POST .../IUD`, `POST .../Fetch` | REST (`GET`, `POST`, `PUT`) + legacy adapters |
| State | `$rootScope`, services | Reactive forms, signal stores, RxJS |
| Validation | Client + server mixed in controllers | FluentValidation + typed form validators |
| Data access | ADO.NET / EF in controllers | Repository pattern, CQRS handlers |
| Auth | Forms auth / session | JWT Bearer (target) |
| Deployment | Monolithic IIS site | SPA static host + API service |

## Migration Phases

### Phase 0 — Discovery and Baseline (Complete for POC)

**Goals:** Understand legacy behavior, identify boundaries, establish target architecture.

| Activity | Status |
|----------|--------|
| Inventory legacy screens and API calls | ✅ POC scope: registration |
| Document data models and dropdown dependencies | ✅ |
| Define target folder structure and tech stack | ✅ |
| Set up repository with frontend + backend skeleton | ✅ |

**Deliverables:** Architecture docs, API contracts, this migration plan.

---

### Phase 1 — Backend API Foundation (Complete for POC)

**Goals:** Stand up .NET 8 API with dual data mode and legacy adapters.

| Activity | Status |
|----------|--------|
| Clean Architecture solution (Domain, Application, Infrastructure, Api) | ✅ |
| `ApiResponse<T>` envelope and exception middleware | ✅ |
| Patient CRUD + search + duplicate check | ✅ |
| Master data / dropdown endpoints | ✅ |
| Legacy adapters (`IUD`, `Fetch`, `FetchPatientData`, `CommonDropdown/Fetch`) | ✅ |
| Mock mode with 5 patients + 17 dropdown types + India geography seed | ✅ |
| SQL Server mode with EF Core seeding | ✅ |

**Exit criteria:** API passes integration tests; Swagger documents all endpoints; Mock mode demoable without database.

---

### Phase 2 — Frontend Shell and Core Services (Complete for POC)

**Goals:** Angular 18 app with auth shell, routing, and shared infrastructure.

| Activity | Status |
|----------|--------|
| Standalone bootstrap, lazy routes, Material theme | ✅ |
| Core layer (interceptors, guards, HTTP services) | ✅ |
| Layout (main layout with sidebar navigation) | ✅ |
| Login and dashboard with hospital-grade UI | ✅ |
| MasterDataService for cascading dropdowns | ✅ |
| Notification and loading services | ✅ |

**Exit criteria:** App loads, authenticates (placeholder), navigates between routes.

---

### Phase 3 — Patient Registration Feature (Complete for POC)

**Goals:** Parity with legacy registration screen for core workflows.

| Activity | Status |
|----------|--------|
| Multi-section registration form (personal, address, professional, additional, documents) | ✅ |
| Dashboard-style UI shell (hero, panels, quick actions) on dashboard, list, and register | ✅ |
| Sticky action bar (New / Search / Save / Reset) | ✅ |
| Patient search dialog (MRN, name, mobile, civil ID) | ✅ |
| Update mode after patient selection | ✅ |
| Age calculation from DOB (years / months / days); gender from title | ✅ |
| Address cascade Country → State → City → Area with India full geography | ✅ |
| Duplicate warning dialog | ✅ |
| Patient list with dashboard-style directory layout | ✅ |

**Exit criteria:** User can search, load, edit, and save a patient end-to-end in Mock mode.

---

### Phase 4 — Database Integration and Parallel Run (Next)

**Goals:** Run new stack against production-like SQL Server data.

| Activity | Status |
|----------|--------|
| EF Core migrations against staging database | 🔲 |
| Data validation scripts (record counts, field mapping) | 🔲 |
| Point frontend to SQL Server backend | 🔲 |
| Side-by-side comparison with legacy (same inputs → same outputs) | 🔲 |
| Performance baseline (search, save latency) | 🔲 |

**Risks:** Schema drift between legacy DB and new EF model. Mitigate with explicit mapping layer and DBA review.

---

### Phase 5 — Authentication and Authorization (Next)

**Goals:** Replace placeholder auth with production-ready security.

| Activity | Status |
|----------|--------|
| Integrate with organizational identity provider (AD / Azure AD) | 🔲 |
| Enable `[Authorize]` on API controllers | 🔲 |
| Role-based route guards (Receptionist, Admin) | 🔲 |
| Secure JWT secret management | 🔲 |
| Audit logging (who created/updated patients) | 🔲 |

---

### Phase 6 — Legacy Decommission (Future)

**Goals:** Retire AngularJS views and MVC controllers for patient registration.

| Activity | Status |
|----------|--------|
| Traffic routing: redirect legacy URLs to new SPA | 🔲 |
| Monitor legacy adapter usage metrics | 🔲 |
| Remove legacy endpoints when traffic reaches zero | 🔲 |
| Decommission AngularJS bundle for registration module | 🔲 |
| Archive MVC controllers and related views | 🔲 |

**Prerequisite:** All hospital sites validated on new stack for registration workflows.

---

### Phase 7 — Expand to Remaining HMS Modules (Future)

**Goals:** Apply the same pattern to other hospital modules.

| Module | Priority |
|--------|----------|
| Appointments / Scheduling | High |
| Admissions | High |
| Billing / Insurance | Medium |
| Laboratory / Radiology orders | Medium |
| Pharmacy | Medium |
| Reporting / Dashboards | Low |

Each module follows Phases 1–6 independently, reusing core infrastructure.

## Strangler-Fig: Legacy Adapter Mapping

During parallel operation, legacy clients continue calling familiar endpoints. Adapters delegate to modern services:

```
Legacy Client                    Adapter Controller              Application Service
     │                                  │                              │
     │  POST /patientregistration/IUD   │                              │
     ├─────────────────────────────────►│  ILegacyPatientRegistration  │
     │                                  ├─────────────────────────────►│
     │                                  │         Create/Update/Delete │
     │                                  │◄─────────────────────────────┤
     │◄─────────────────────────────────┤     ApiResponse<T>           │
```

| Legacy endpoint | Modern handler |
|-----------------|----------------|
| `POST /api/patientregistration/IUD` | `CreatePatientCommand` / `UpdatePatientCommand` / `DeletePatientCommand` |
| `POST /api/patientregistration/Fetch` | `SearchPatientsByCriteriaQuery` |
| `POST /api/patientregistration/FetchPatientData` | `GetPatientByIdQuery` |
| `POST /api/CommonDropdown/Fetch` | `GetMasterDataByTypeQuery` |

No business logic lives in adapter controllers.

## Frontend Migration Patterns

### AngularJS → Angular

| Legacy pattern | Target pattern |
|----------------|----------------|
| `$scope` binding | Reactive forms + `FormControl` |
| `ng-repeat` | `@for` control flow |
| `$http.post('/api/.../Fetch')` | `HttpClient` via `ApiBaseService` |
| Directives | Standalone components |
| `angular.module()` | `bootstrapApplication()` + `provideRouter()` |
| UI Bootstrap | Angular Material |

### Recommended screen migration order

1. **Patient Registration** — highest daily use (✅ POC complete)
2. **Patient Search / List** — read-heavy, validates search API (✅ dashboard-style list complete)
3. **Patient Detail / Edit** — extends registration patterns (🔲 partial)
4. **Reports** — lower risk, read-only
5. **Admin / Configuration** — lowest urgency

## Data Migration Considerations

### Mock → SQL Server

The POC uses identical seed data in both modes (`MockSeedData` + `IndiaGeographySeed`). For production:

1. Map legacy table columns to new EF entities
2. Write one-time migration scripts for historical patients
3. Preserve legacy IDs in `LegacyPatientId` field for traceability
4. Validate MR Number format continuity (`MRN-YYYY-NNNNNN`)

### Dropdown migration

Legacy `CommonDropdown/Fetch` may return different shapes per type. The new API normalizes to `MasterDataItemDto`. During parallel run:

- Compare dropdown item counts per type
- Verify code/display name mappings with clinical staff
- Load production India state/city master data (POC uses `IndiaGeographySeed` with all states/UTs and major cities)
- Add missing types to `MasterDataType` enum as discovered

## Testing Strategy

| Layer | Approach |
|-------|----------|
| Backend unit | Handler and validator tests (MediatR) |
| Backend integration | WebApplicationFactory against Mock mode |
| Frontend unit | Component + service tests (Jasmine/Karma) |
| E2E | Playwright/Cypress for registration happy path |
| Regression | Side-by-side legacy vs new output comparison |
| UAT | Hospital reception staff on staging environment |

## Rollback Plan

| Scenario | Action |
|----------|--------|
| New API failure in production | Route traffic back to MVC endpoints via reverse proxy |
| Data corruption on save | Disable write endpoints; legacy system remains source of truth |
| Frontend defect | Revert SPA deployment; legacy AngularJS still served from IIS |
| Partial migration | Legacy adapters keep working independently of new UI |

Maintain legacy system operable until Phase 6 sign-off.

## Risks and Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Schema mismatch | Data loss / corruption | Mapping layer, staged migration, DBA review |
| Duplicate patient logic differences | Clinical safety | Parallel duplicate-check validation, clinician sign-off |
| Staff training | Adoption resistance | Material UI similar workflows, phased rollout per site |
| Hidden legacy API consumers | Broken integrations | Monitor adapter logs, inventory all callers before decommission |
| Performance regression | Queue delays at reception | Load test search and save; index MR Number, NationalId |
| Scope creep | Delayed delivery | Strict module boundaries, POC-first per feature |

## Future Enhancements

### Short term (3–6 months)

- Enable production JWT authentication and role policies
- Connect frontend search dialog to live API (`useMockPatientSearch: false`) — default in POC
- EF Core migrations against staging/production schema
- Unit and integration test coverage for critical paths
- CI pipeline (build, lint, test on pull request)

### Medium term (6–12 months)

- Complete patient detail and edit feature parity (list and register UI done)
- Document upload to blob storage (Azure Blob / S3)
- Real-time duplicate check debouncing on form fields
- Localization (Arabic / English) for Gulf hospital deployment
- Accessibility audit (WCAG 2.1 AA)

### Long term (12+ months)

- Remaining HMS modules (appointments, admissions, billing)
- Event-driven architecture (patient created → downstream systems)
- API versioning (`/api/v1/`, `/api/v2/`)
- Kubernetes deployment with health probes and autoscaling
- Remove all legacy MVC controllers and AngularJS bundles
- Centralized observability (Application Insights, structured logging, dashboards)

## Success Criteria

The migration is considered successful for Patient Registration when:

1. All legacy registration workflows are available in the Angular app
2. Data reads/writes are validated against SQL Server production schema
3. No increase in duplicate patient creation rate post-migration
4. Reception staff UAT sign-off at pilot site
5. Legacy registration traffic reduced to zero for 30 consecutive days
6. Legacy endpoints decommissioned with no downstream breakage

## References

- [README.md](../README.md) — Quick start and repository overview
- [ARCHITECTURE.md](ARCHITECTURE.md) — Technical architecture and data modes
- [API_DOCUMENTATION.md](API_DOCUMENTATION.md) — Endpoint reference
