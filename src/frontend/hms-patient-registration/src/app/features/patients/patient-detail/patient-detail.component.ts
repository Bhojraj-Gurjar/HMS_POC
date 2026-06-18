import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { DatePipe } from '@angular/common';
import { finalize } from 'rxjs';
import { PatientService } from '../../../core/services/patient.service';
import { LoadingService } from '../../../core/services/loading.service';
import { PatientDetail } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-detail',
  standalone: true,
  imports: [RouterLink, MatCardModule, MatButtonModule, MatChipsModule, DatePipe],
  template: `
    @if (patient(); as p) {
      <div class="detail-page">
        <header class="page-header">
          <div>
            <h1>{{ p.fullName }}</h1>
            <p>{{ p.patientNumber }}</p>
          </div>
          <a mat-flat-button [routerLink]="['/patients', p.id, 'edit']">Edit Patient</a>
        </header>

        <mat-card>
          <mat-card-content class="detail-grid">
            <div><span class="label">Status</span><mat-chip>{{ p.status }}</mat-chip></div>
            <div><span class="label">Gender</span>{{ p.gender }}</div>
            <div><span class="label">DOB</span>{{ p.dateOfBirth | date: 'mediumDate' }}</div>
            <div><span class="label">Blood Group</span>{{ p.bloodGroup }}</div>
            <div><span class="label">Phone</span>{{ phone }}</div>
            <div><span class="label">Email</span>{{ email }}</div>
            <div class="full"><span class="label">Address</span>{{ address }}</div>
            <div class="full"><span class="label">Notes</span>{{ p.notes || '—' }}</div>
          </mat-card-content>
        </mat-card>
      </div>
    }
  `,
  styles: `
    .detail-page {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }

    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 16px;

      h1 {
        margin: 0;
      }

      p {
        margin: 4px 0 0;
        color: #717182;
      }
    }

    .detail-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 16px;
    }

    .full {
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
export class PatientDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly patientService = inject(PatientService);
  private readonly loading = inject(LoadingService);

  readonly patient = signal<PatientDetail | null>(null);

  get phone(): string {
    return this.patient()?.contacts.find((c) => c.contactType === 'Phone')?.value ?? '—';
  }

  get email(): string {
    return this.patient()?.contacts.find((c) => c.contactType === 'Email')?.value ?? '—';
  }

  get address(): string {
    const p = this.patient();
    const addr = p?.addresses.find((a) => a.isPrimary) ?? p?.addresses[0];
    if (!addr) return '—';
    return [addr.line1, addr.city, addr.state, addr.country].filter(Boolean).join(', ');
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.loading.show();
    this.patientService
      .getById(id)
      .pipe(finalize(() => this.loading.hide()))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.patient.set(response.data);
          }
        },
      });
  }
}
