import { Routes } from '@angular/router';

export const PATIENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./patient-list/patient-list.component').then((m) => m.PatientListComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./patient-registration/patient-registration.component').then(
        (m) => m.PatientRegistrationComponent,
      ),
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./patient-detail/patient-detail.component').then((m) => m.PatientDetailComponent),
  },
  {
    path: ':id/edit',
    loadComponent: () =>
      import('./patient-edit/patient-edit.component').then((m) => m.PatientEditComponent),
  },
];
