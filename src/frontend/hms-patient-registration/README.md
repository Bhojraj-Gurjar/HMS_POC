# HMS Patient Registration — Frontend

Angular 18 standalone application for the HMS Patient Registration Migration POC.

## Prerequisites

- [Node.js 20+](https://nodejs.org/)
- Backend API running at `https://localhost:7001` (or use proxy)

## Setup

```bash
cd src/frontend/hms-patient-registration
npm install
npm start
```

App: `http://localhost:4200`

**Demo login:** any credentials work (e.g. `admin` / `admin123`).

## Scripts

| Command | Description |
|---------|-------------|
| `npm start` | Dev server with API proxy |
| `npm run build` | Production build |
| `npm run lint` | ESLint |
| `npm run format` | Prettier |

## Architecture

```
src/app/
├── core/           Auth, guards, interceptors, API base, dropdown service
├── shared/         Reusable UI, pipes, validators
├── layout/         Shell (header, sidenav)
└── features/
    ├── auth/       Login
    ├── dashboard/  Overview (hero, KPIs, recent patients)
    └── patients/   List, registration, detail, edit

src/styles/
└── _page-layout.scss   Shared hms-* dashboard-style layout classes
```

## Routes

| Path | Screen |
|------|--------|
| `/login` | Authentication |
| `/dashboard` | Hospital overview |
| `/patients` | Patient directory (search + table) |
| `/patients/register` | Registration form (search, create, update) |
| `/patients/:id` | Patient detail |
| `/patients/:id/edit` | Patient edit |

## UI Layout

Dashboard, patient list, and registration share the **`hms-*` layout system** (`src/styles/_page-layout.scss`):

- Blue gradient **hero** banner with primary actions
- **Stat cards** on dashboard and patient list (not on registration)
- **Two-column panels** — main content + quick-actions sidebar on list/register
- **Single-column** reactive form fields within section cards

## Registration Form

| Section | Fields (highlights) |
|---------|---------------------|
| Setup strip | Patient type toggle, appointment reference |
| Personal | Name, DOB, age Y/M/D, gender, blood group, mobile, email |
| Address | Type, lines, Country → State → City → Area, pincode |
| Professional | Occupation, company, profession, income |
| Additional | Nationality, emergency contacts, insurance |
| Documents | Upload placeholders |

**Note:** Mobile phone is captured in personal details only (not duplicated on address). India address cascade loads all 36 states/UTs and cities per state from the API.

## API Integration

All HTTP calls unwrap the backend `ApiResponse<T>` envelope via `ApiBaseService`.

| File | Purpose |
|------|---------|
| `environment.ts` | `apiUrl`, `useMockPatientSearch` |
| `proxy.conf.json` | Dev proxy `/api` → `https://localhost:7001` |

`DropdownService` caches master data and supports `refresh(category, parentCode)` for cascading address dropdowns.

## State Management

- **Registration** — `PatientRegistrationFormService` + typed reactive `FormGroup` (no global store)
- **Patient list** — component signals + `PatientApiService`
- **Wizard** — `PatientRegistrationStore` exists for legacy wizard (not routed in POC)
