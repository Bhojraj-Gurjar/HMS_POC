import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { AuthService } from '../../core/services/auth.service';

interface NavItem {
  path: string;
  icon: string;
  label: string;
}

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatButtonModule,
    MatIconModule,
    MatBadgeModule,
    MatSidenavModule,
    MatToolbarModule,
  ],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent {
  private readonly auth = inject(AuthService);
  private readonly breakpointObserver = inject(BreakpointObserver);

  readonly sidenavOpen = signal(false);
  readonly isHandset = signal(false);

  readonly menuItems: NavItem[] = [
    { path: '/dashboard', icon: 'dashboard', label: 'Overview' },
    { path: '/patients/register', icon: 'person_add', label: 'Register Patient' },
    { path: '/patients', icon: 'groups', label: 'Patients List' },
    { path: '/appointments', icon: 'event', label: 'Appointments' },
    { path: '/reports', icon: 'description', label: 'Reports' },
  ];

  readonly username = this.auth.user()?.username ?? 'Admin User';
  readonly userInitials = this.auth.getUserInitials();

  constructor() {
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe((result) => {
      this.isHandset.set(result.matches);
      if (!result.matches) {
        this.sidenavOpen.set(false);
      }
    });
  }

  toggleSidenav(): void {
    this.sidenavOpen.update((open) => !open);
  }

  closeSidenav(): void {
    if (this.isHandset()) {
      this.sidenavOpen.set(false);
    }
  }

  logout(): void {
    this.auth.logout();
  }
}
