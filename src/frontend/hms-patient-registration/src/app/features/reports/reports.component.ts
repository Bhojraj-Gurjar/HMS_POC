import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatCardModule, MatIconModule],
  template: `
    <div class="hms-page">
      <section class="hms-hero">
        <div>
          <p class="hms-hero__eyebrow">Analytics</p>
          <h1>Reports</h1>
          <p class="hms-hero__subtitle">Generate patient census, admission trends, and departmental reports.</p>
        </div>
        <div class="hms-hero__actions">
          <a mat-stroked-button routerLink="/dashboard" class="hms-hero-btn hms-hero-btn--light">
            <mat-icon>dashboard</mat-icon>
            Dashboard
          </a>
        </div>
      </section>
      <mat-card class="placeholder-card">
        <mat-card-content>
          <p>Analytics and reporting module coming soon.</p>
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
export class ReportsComponent {}
