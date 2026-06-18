import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [RouterLink, MatButtonModule],
  template: `
    <div class="unauthorized">
      <h1>Unauthorized</h1>
      <p>You do not have permission to access this page.</p>
      <a mat-flat-button color="primary" routerLink="/dashboard">Go to Dashboard</a>
    </div>
  `,
  styles: `
    .unauthorized {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 16px;
      min-height: 60vh;
      text-align: center;
    }
  `,
})
export class UnauthorizedComponent {}
