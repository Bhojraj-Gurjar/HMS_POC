import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatCardModule, MatIconModule],
  template: `
    <div class="hms-page">
      <section class="hms-hero">
        <div>
          <p class="hms-hero__eyebrow">Scheduling</p>
          <h1>Appointments</h1>
          <p class="hms-hero__subtitle">Manage doctor schedules, patient bookings, and follow-ups.</p>
        </div>
        <div class="hms-hero__actions">
          <a mat-stroked-button routerLink="/patients/register" class="hms-hero-btn hms-hero-btn--light">
            <mat-icon>person_add</mat-icon>
            Register
          </a>
        </div>
      </section>
      <mat-card class="placeholder-card">
        <mat-card-content>
          <p>Appointment scheduling module coming soon.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: `
    .placeholder-card {
      border-radius: 12px;
      border: 1px solid rgba(0, 0, 0, 0.06);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
    }

    p {
      margin: 0;
      color: #717182;
    }
  `,
})
export class AppointmentsComponent {}
