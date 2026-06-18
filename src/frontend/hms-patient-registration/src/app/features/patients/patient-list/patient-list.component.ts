import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs';
import { PatientService } from '../../../core/services/patient.service';
import { LoadingService } from '../../../core/services/loading.service';
import { PatientSummary } from '../../../core/models/patient.model';
import { PatientDetailDialogComponent } from './patient-detail-dialog.component';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [
    RouterLink,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
    MatChipsModule,
    MatDialogModule,
    DatePipe,
  ],
  templateUrl: './patient-list.component.html',
  styleUrl: './patient-list.component.scss',
})
export class PatientListComponent implements OnInit {
  private readonly patientService = inject(PatientService);
  private readonly loading = inject(LoadingService);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);

  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly statusFilter = new FormControl('all', { nonNullable: true });
  readonly patients = signal<PatientSummary[]>([]);
  readonly filteredPatients = signal<PatientSummary[]>([]);
  readonly displayedColumns = ['patientNumber', 'fullName', 'dateOfBirth', 'gender', 'status', 'actions'];

  ngOnInit(): void {
    this.loadPatients();
    this.searchControl.valueChanges.pipe(debounceTime(300), distinctUntilChanged()).subscribe(() => {
      this.applyFilters();
    });
    this.statusFilter.valueChanges.subscribe(() => this.applyFilters());
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map((part) => part[0])
      .join('')
      .slice(0, 2)
      .toUpperCase();
  }

  statusClass(status: string): string {
    if (status === 'Critical') return 'status-critical';
    if (status === 'Active') return 'status-active';
    return 'status-default';
  }

  viewPatient(patient: PatientSummary): void {
    this.loading.show();
    this.patientService
      .getById(patient.id)
      .pipe(finalize(() => this.loading.hide()))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.dialog.open(PatientDetailDialogComponent, {
              data: response.data,
              width: '640px',
            });
          }
        },
      });
  }

  editPatient(id: string): void {
    void this.router.navigate(['/patients', id, 'edit']);
  }

  private loadPatients(): void {
    this.loading.show();
    this.patientService
      .search(undefined, 1, 100)
      .pipe(finalize(() => this.loading.hide()))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.patients.set(response.data.items);
            this.applyFilters();
          }
        },
      });
  }

  private applyFilters(): void {
    const query = this.searchControl.value.toLowerCase().trim();
    const status = this.statusFilter.value;

    this.filteredPatients.set(
      this.patients().filter((patient) => {
        const matchesSearch =
          !query ||
          patient.fullName.toLowerCase().includes(query) ||
          patient.patientNumber.toLowerCase().includes(query) ||
          (patient.primaryPhone ?? '').includes(query);

        const matchesStatus = status === 'all' || patient.status === status;
        return matchesSearch && matchesStatus;
      }),
    );
  }
}
