import { Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DatePipe } from '@angular/common';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs';
import { PatientService } from '../../../core/services/patient.service';
import { PatientSummary } from '../../../core/models/patient.model';

@Component({
  selector: 'app-patient-search-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatProgressSpinnerModule,
    DatePipe,
  ],
  template: `
    <h2 mat-dialog-title>Search Existing Patient</h2>
    <mat-dialog-content>
      <p class="hint">Search by MR number, name, mobile, or civil ID.</p>
      <mat-form-field appearance="outline" class="full-width">
        <mat-label>Search</mat-label>
        <mat-icon matPrefix>search</mat-icon>
        <input matInput [formControl]="searchControl" placeholder="MRN, name, mobile, civil ID..." />
      </mat-form-field>

      @if (loading()) {
        <div class="loading"><mat-spinner diameter="32" /></div>
      } @else if (results().length === 0 && searchControl.value) {
        <p class="empty">No patients found.</p>
      } @else {
        <div class="results">
          @for (patient of results(); track patient.id) {
            <button type="button" class="result-row" (click)="select(patient)">
              <div>
                <strong>{{ patient.fullName }}</strong>
                <span>{{ patient.patientNumber }}</span>
              </div>
              <div class="meta">
                <span>{{ patient.dateOfBirth | date: 'mediumDate' }}</span>
                <span>{{ patient.primaryPhone ?? '—' }}</span>
              </div>
            </button>
          }
        </div>
      }
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
    </mat-dialog-actions>
  `,
  styles: `
    .hint {
      margin: 0 0 12px;
      color: #717182;
      font-size: 0.9rem;
    }

    .full-width {
      width: 100%;
    }

    .loading,
    .empty {
      text-align: center;
      padding: 24px;
      color: #717182;
    }

    .results {
      display: flex;
      flex-direction: column;
      gap: 8px;
      max-height: 320px;
      overflow-y: auto;
    }

    .result-row {
      display: flex;
      justify-content: space-between;
      gap: 12px;
      width: 100%;
      padding: 12px;
      border: 1px solid rgba(0, 0, 0, 0.08);
      border-radius: 8px;
      background: #fff;
      cursor: pointer;
      text-align: left;

      &:hover {
        background: #eff6ff;
        border-color: #93c5fd;
      }

      strong {
        display: block;
        color: #111827;
      }

      span {
        font-size: 0.85rem;
        color: #717182;
      }
    }

    .meta {
      display: flex;
      flex-direction: column;
      align-items: flex-end;
      gap: 4px;
      font-size: 0.85rem;
      color: #717182;
    }

    mat-icon[matPrefix] {
      margin-right: 8px;
      color: #9ca3af;
    }
  `,
})
export class PatientSearchDialogComponent {
  private readonly patientService = inject(PatientService);
  private readonly dialogRef = inject(MatDialogRef<PatientSearchDialogComponent>);

  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly results = signal<PatientSummary[]>([]);
  readonly loading = signal(false);

  constructor() {
    this.searchControl.valueChanges.pipe(debounceTime(300), distinctUntilChanged()).subscribe((term) => {
      const query = term.trim();
      if (!query) {
        this.results.set([]);
        return;
      }
      this.loading.set(true);
      this.patientService
        .search(query, 1, 20)
        .pipe(finalize(() => this.loading.set(false)))
        .subscribe({
          next: (response) => this.results.set(response.data?.items ?? []),
        });
    });
  }

  select(patient: PatientSummary): void {
    this.dialogRef.close(patient);
  }
}
