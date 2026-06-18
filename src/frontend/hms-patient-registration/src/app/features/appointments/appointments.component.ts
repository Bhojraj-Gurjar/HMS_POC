import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [MatCardModule, MatIconModule],
  template: `
    <div class="placeholder-page">
      <div class="placeholder-icon">
        <mat-icon>event</mat-icon>
      </div>
      <h1>Appointments</h1>
      <p>Appointment scheduling module coming soon.</p>
      <mat-card>
        <mat-card-content>
          <p>This section will manage doctor schedules, patient bookings, and follow-ups.</p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: `
    .placeholder-page {
      display: flex;
      flex-direction: column;
      gap: 16px;
      max-width: 640px;
    }

    .placeholder-icon {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 48px;
      height: 48px;
      border-radius: 12px;
      background: linear-gradient(135deg, #a855f7, #9333ea);
      color: #fff;
    }

    h1 {
      margin: 0;
      font-size: 1.875rem;
    }

    p {
      margin: 0;
      color: #717182;
    }
  `,
})
export class AppointmentsComponent {}
