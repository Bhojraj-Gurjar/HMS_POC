import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { finalize } from 'rxjs';
import { PatientService } from '../../../core/services/patient.service';
import { LoadingService } from '../../../core/services/loading.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-patient-edit',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Edit Patient</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="save()" class="edit-form">
          <mat-form-field appearance="outline">
            <mat-label>First Name</mat-label>
            <input matInput formControlName="firstName" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Last Name</mat-label>
            <input matInput formControlName="lastName" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Date of Birth</mat-label>
            <input matInput type="date" formControlName="dateOfBirth" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Gender</mat-label>
            <mat-select formControlName="gender">
              <mat-option value="Male">Male</mat-option>
              <mat-option value="Female">Female</mat-option>
              <mat-option value="Other">Other</mat-option>
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Phone</mat-label>
            <input matInput formControlName="phone" />
          </mat-form-field>
          <div class="actions">
            <button mat-flat-button color="primary" type="submit">Save Changes</button>
            <button mat-button type="button" (click)="cancel()">Cancel</button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: `
    .edit-form {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 16px;
      margin-top: 16px;
    }

    .actions {
      grid-column: 1 / -1;
      display: flex;
      gap: 12px;
    }
  `,
})
export class PatientEditComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly patientService = inject(PatientService);
  private readonly loading = inject(LoadingService);
  private readonly notifications = inject(NotificationService);

  private patientId = '';

  readonly form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    dateOfBirth: ['', Validators.required],
    gender: ['', Validators.required],
    phone: ['', Validators.required],
    bloodGroup: [''],
    address: [''],
    email: [''],
  });

  ngOnInit(): void {
    this.patientId = this.route.snapshot.paramMap.get('id') ?? '';
    if (!this.patientId) return;

    this.loading.show();
    this.patientService
      .getById(this.patientId)
      .pipe(finalize(() => this.loading.hide()))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            const p = response.data;
            this.form.patchValue({
              firstName: p.firstName,
              lastName: p.lastName,
              dateOfBirth: p.dateOfBirth,
              gender: p.gender,
              phone: p.contacts.find((c) => c.contactType === 'Phone')?.value ?? '',
              email: p.contacts.find((c) => c.contactType === 'Email')?.value ?? '',
              bloodGroup: p.bloodGroup,
              address: p.addresses[0]?.line1 ?? '',
            });
          }
        },
      });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.loading.show();
    this.patientService
      .update(this.patientId, {
        firstName: value.firstName,
        lastName: value.lastName,
        dateOfBirth: value.dateOfBirth,
        gender: value.gender,
        bloodGroup: value.bloodGroup || 'Unknown',
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
        emergencyContacts: [],
        insurances: [],
      })
      .pipe(finalize(() => this.loading.hide()))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notifications.success('Patient updated successfully');
            this.navigateBack();
          }
        },
      });
  }

  cancel(): void {
    this.navigateBack();
  }

  private navigateBack(): void {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/patients';
    void this.router.navigateByUrl(returnUrl);
  }
}
