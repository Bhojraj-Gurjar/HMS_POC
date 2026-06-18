import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { AuthUser, LoginRequest, LoginResponse } from '../models/auth.model';

const STORAGE_KEY = 'hms_auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly userSignal = signal<AuthUser | null>(this.readStoredUser());

  readonly user = this.userSignal.asReadonly();

  isAuthenticated(): boolean {
    return !!this.userSignal()?.token;
  }

  login(request: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${environment.apiUrl}/auth/login`, request)
      .pipe(
        tap((response) => {
          if (response.success && response.data) {
            const authUser: AuthUser = {
              username: response.data.username,
              roles: response.data.roles,
              token: response.data.accessToken,
            };
            localStorage.setItem(STORAGE_KEY, JSON.stringify(authUser));
            this.userSignal.set(authUser);
          }
        }),
      );
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.userSignal.set(null);
    void this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.userSignal()?.token ?? null;
  }

  getUserInitials(): string {
    const username = this.userSignal()?.username ?? 'AD';
    return username.slice(0, 2).toUpperCase();
  }

  private readStoredUser(): AuthUser | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return raw ? (JSON.parse(raw) as AuthUser) : null;
    } catch {
      return null;
    }
  }
}
