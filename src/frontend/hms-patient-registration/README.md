# HMS Patient Registration — Frontend

Angular 18 standalone application for the HMS Patient Registration Migration POC.

## Setup

```bash
cd src/frontend/hms-patient-registration
npm install
npm start
```

App: `http://localhost:4200`

**Login:** `admin` / `admin123`

Backend must be running at `http://localhost:5001` (dev proxy forwards `/api`).

## Scripts

| Command | Description |
|---------|-------------|
| `npm start` | Dev server + API proxy |
| `npm run build` | Production build |
| `npm run lint` | ESLint |
| `npm run format` | Prettier |

## Structure

```
src/app/
├── core/
│   ├── auth/              Guards
│   ├── interceptors/      Auth, correlation ID, error handling
│   ├── models/            ApiResponse, patient, auth types
│   └── services/          auth, patient, master-data, loading, notification
├── shared/ui/             Loading overlay
├── layout/main-layout/    Sidebar, header, navigation
└── features/
    ├── auth/              Login, unauthorized
    ├── dashboard/         KPIs, charts, recent patients
    ├── patients/          List, registration, detail, edit
    ├── appointments/      Placeholder
    └── reports/           Placeholder

src/styles/_page-layout.scss   Shared hms-* layout (hero, panels)
```

## Routes

| Path | Screen |
|------|--------|
| `/login` | Authentication |
| `/dashboard` | Hospital overview |
| `/patients` | Patient directory |
| `/patients/register` | Registration (create / update) |
| `/patients/:id` | Patient detail |
| `/appointments` | Appointments placeholder |
| `/reports` | Reports placeholder |

## UI

All main screens use the shared **`hms-hero`** blue gradient header (`_page-layout.scss`):

- Eyebrow label, title, subtitle
- Outlined action buttons on the right
- Consistent spacing via `hms-page` wrapper

## Registration

| Section | Highlights |
|---------|------------|
| Setup | Patient type (new / existing / staff / newborn), appointment reference |
| Demographics | Name, DOB, auto age, gender, blood group, mobile, email |
| Address | Country → State → City → Area cascade |
| Professional | Occupation, company, profession, income |
| Insurance | Placeholder fields |
| Documents | PDF/JPG/PNG upload (max 5MB), preview, stored in `registrationMeta` JSON |
| Sidebar | Recent patients with colored status chips |

## API Integration

| Service | Endpoints used |
|---------|----------------|
| `PatientService` | `/patients`, `/patients/duplicate-check`, `/dashboard/stats` |
| `MasterDataService` | `/masterdata/{type}?parentCode=` |
| `AuthService` | `/auth/login` |

| File | Purpose |
|------|---------|
| `environment.ts` | `apiUrl: '/api'` |
| `proxy.conf.json` | `/api` → `http://localhost:5001` |

HTTP interceptors attach JWT and correlation ID. Responses use the backend `ApiResponse<T>` envelope.
