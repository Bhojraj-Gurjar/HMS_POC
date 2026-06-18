import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoadingOverlayComponent } from './shared/ui/loading-overlay/loading-overlay.component';
import { LoadingService } from './core/services/loading.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, LoadingOverlayComponent],
  template: `
    <router-outlet />
    <app-loading-overlay [visible]="loadingService.isLoading()" />
  `,
})
export class AppComponent {
  constructor(readonly loadingService: LoadingService) {}
}
