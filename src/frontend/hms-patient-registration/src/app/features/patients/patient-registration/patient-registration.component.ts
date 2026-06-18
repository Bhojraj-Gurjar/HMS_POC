import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { finalize } from 'rxjs';
import { PatientService } from '../../../core/services/patient.service';
import { LoadingService } from '../../../core/services/loading.service';
import { NotificationService } from '../../../core/services/notification.service';
import { CreatePatientRequest } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-registration',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
  ],
  templateUrl: './patient-registration.component.html',
  styleUrl: './patient-registration.component.scss',
})
export class PatientRegistrationComponent {
  private readonly fb = inject(FormBuilder);
  private readonly patientService = inject(PatientService);
  private readonly router = inject(Router);
  private readonly loading = inject(LoadingService);
  private readonly notifications = inject(NotificationService);

  readonly bloodGroups = ['A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-'];
  readonly genders = ['Male', 'Female', 'Other'];

  readonly form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    dateOfBirth: ['', Validators.required],
    gender: ['', Validators.required],
    bloodGroup: [''],
    phone: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    email: ['', Validators.email],
    address: [''],
    emergencyContact: [''],
    emergencyName: [''],
    notes: [''],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.error('Please fill in all required fields');
      return;
    }

    const value = this.form.getRawValue();
    const request: CreatePatientRequest = {
      firstName: value.firstName,
      lastName: value.lastName,
      dateOfBirth: value.dateOfBirth,
      gender: value.gender,
      bloodGroup: value.bloodGroup || 'Unknown',
      notes: value.notes || null,
      addresses: value.address
        ? [
            {
              addressType: 'Home',
              line1: value.address,
              city: 'Unknown',
              country: 'India',
              isPrimary: true,
            },
          ]
        : [],
      contacts: [
        { contactType: 'Phone', value: value.phone, isPrimary: true },
        ...(value.email ? [{ contactType: 'Email', value: value.email, isPrimary: false }] : []),
      ],
      emergencyContacts: value.emergencyContact
        ? [
            {
              name: value.emergencyName || 'Emergency Contact',
              relationship: 'Other',
              phone: value.emergencyContact,
              isPrimary: true,
            },
          ]
        : [],
      insurances: [],
    };

    this.loading.show();
    this.patientService
      .create(request)
      .pipe(finalize(() => this.loading.hide()))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notifications.success('Patient registered successfully!');
            void this.router.navigate(['/patients']);
          }
        },
      });
  }

  reset(): void {
    this.form.reset();
  }
}
