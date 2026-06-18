import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { AuthService } from '../../core/services/auth.service';

interface NavItem {
  path: string;
  icon: string;
  label: string;
  exact: boolean;
  activeClass: string;
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
    { path: '/dashboard', icon: 'dashboard', label: 'Overview', exact: true, activeClass: 'nav-link--active-overview' },
    { path: '/patients/register', icon: 'person_add', label: 'Register Patient', exact: true, activeClass: 'nav-link--active-register' },
    { path: '/patients', icon: 'groups', label: 'Patients List', exact: true, activeClass: 'nav-link--active-patients' },
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
    this.closeSidenav();
    this.auth.logout();
  }
}
