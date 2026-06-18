import { Component, Input } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-overlay',
  standalone: true,
  imports: [MatProgressSpinnerModule],
  template: `
    @if (visible) {
      <div class="loading-overlay" aria-live="polite" aria-busy="true">
        <mat-spinner diameter="48" />
      </div>
    }
  `,
  styles: `
    .loading-overlay {
      position: fixed;
      inset: 0;
      z-index: 9999;
      display: flex;
      align-items: center;
      justify-content: center;
      background: rgba(255, 255, 255, 0.65);
      backdrop-filter: blur(2px);
    }
  `,
})
export class LoadingOverlayComponent {
  @Input() visible = false;
}
