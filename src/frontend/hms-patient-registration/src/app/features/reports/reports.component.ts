import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [MatCardModule, MatIconModule],
  template: `
    <div class="placeholder-page">
      <div class="placeholder-icon">
        <mat-icon>description</mat-icon>
      </div>
      <h1>Reports</h1>
      <p>Analytics and reporting module coming soon.</p>
      <mat-card>
        <mat-card-content>
          <p>Generate patient census, admission trends, and departmental reports from here.</p>
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
      background: linear-gradient(135deg, #3b82f6, #2563eb);
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
export class ReportsComponent {}
