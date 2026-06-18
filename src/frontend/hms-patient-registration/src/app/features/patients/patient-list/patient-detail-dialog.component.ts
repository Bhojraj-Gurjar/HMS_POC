import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { DatePipe } from '@angular/common';
import { PatientDetail } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-detail-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatChipsModule, DatePipe],
  template: `
    <h2 mat-dialog-title>Patient Details</h2>
    <mat-dialog-content>
      <p class="subtitle">Complete information for {{ data.fullName }}</p>
      <div class="detail-grid">
        <div>
          <span class="label">Patient ID</span>
          <span>{{ data.patientNumber }}</span>
        </div>
        <div>
          <span class="label">Blood Group</span>
          <span>{{ data.bloodGroup || '—' }}</span>
        </div>
        <div>
          <span class="label">Gender</span>
          <span>{{ data.gender }}</span>
        </div>
        <div>
          <span class="label">Date of Birth</span>
          <span>{{ data.dateOfBirth | date: 'mediumDate' }}</span>
        </div>
        <div>
          <span class="label">Phone</span>
          <span>{{ primaryPhone }}</span>
        </div>
        <div>
          <span class="label">Email</span>
          <span>{{ emailContact }}</span>
        </div>
        <div class="full-width">
          <span class="label">Address</span>
          <span>{{ primaryAddress }}</span>
        </div>
        <div>
          <span class="label">Registered</span>
          <span>{{ data.createdAt | date: 'medium' }}</span>
        </div>
        <div>
          <span class="label">Status</span>
          <mat-chip>{{ data.status }}</mat-chip>
        </div>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Close</button>
    </mat-dialog-actions>
  `,
  styles: `
    .subtitle {
      margin: 0 0 16px;
      color: #717182;
    }

    .detail-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 16px;
    }

    .full-width {
      grid-column: 1 / -1;
    }

    .label {
      display: block;
      font-size: 0.8rem;
      font-weight: 600;
      color: #717182;
      margin-bottom: 4px;
    }
  `,
})
export class PatientDetailDialogComponent {
  readonly data = inject<PatientDetail>(MAT_DIALOG_DATA);

  get primaryPhone(): string {
    return this.data.contacts.find((c) => c.contactType === 'Phone')?.value ?? '—';
  }

  get emailContact(): string {
    return this.data.contacts.find((c) => c.contactType === 'Email')?.value ?? '—';
  }

  get primaryAddress(): string {
    const address = this.data.addresses.find((a) => a.isPrimary) ?? this.data.addresses[0];
    if (!address) return '—';
    return [address.line1, address.city, address.state, address.country].filter(Boolean).join(', ');
  }
}
