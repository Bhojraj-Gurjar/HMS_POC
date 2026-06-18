import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly _loading = signal(false);
  private depth = 0;

  isLoading(): boolean {
    return this._loading();
  }

  show(): void {
    this.depth += 1;
    this._loading.set(true);
  }

  hide(): void {
    this.depth = Math.max(0, this.depth - 1);
    if (this.depth === 0) {
      this._loading.set(false);
    }
  }
}
