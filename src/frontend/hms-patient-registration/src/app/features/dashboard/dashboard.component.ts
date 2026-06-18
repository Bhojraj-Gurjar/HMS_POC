import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { DatePipe, NgClass } from '@angular/common';
import { PatientService } from '../../core/services/patient.service';
import { LoadingService } from '../../core/services/loading.service';
import { DashboardStats, PatientSummary } from '../../core/models/patient.model';
import { catchError, finalize, map, of } from 'rxjs';

interface StatCard {
  title: string;
  value: string;
  change: string;
  trend: 'up' | 'down';
  icon: string;
  colorClass: string;
  route: string;
}

interface AppointmentPreview {
  patientName: string;
  doctorName: string;
  department: string;
  time: string;
  date: string;
  type: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    DatePipe,
    NgClass,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private readonly patientService = inject(PatientService);
  private readonly loading = inject(LoadingService);

  readonly stats = signal<StatCard[]>([]);
  readonly weeklyData = signal<{ day: string; patients: number; appointments: number }[]>([]);
  readonly departmentData = signal<{ name: string; value: number; color: string }[]>([]);
  readonly recentPatients = signal<PatientSummary[]>([]);
  readonly upcomingAppointments = signal<AppointmentPreview[]>([]);
  readonly maxWeeklyValue = signal(1);

  private readonly chartColors = ['#3b82f6', '#8b5cf6', '#06b6d4', '#10b981', '#f59e0b'];

  ngOnInit(): void {
    this.loading.show();
    this.patientService
      .getDashboardStats()
      .pipe(
        catchError(() =>
          this.patientService.search(undefined, 1, 100).pipe(
            map((response) => ({
              success: true,
              message: null,
              data: this.buildFallbackStats(response.data?.items ?? []),
              errors: null,
              correlationId: null,
            })),
            catchError(() =>
              of({
                success: true,
                message: null,
                data: this.buildFallbackStats([]),
                errors: null,
                correlationId: null,
              }),
            ),
          ),
        ),
        finalize(() => this.loading.hide()),
      )
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.applyStats(response.data);
          }
        },
      });
  }

  private buildFallbackStats(patients: PatientSummary[]): DashboardStats {
    const now = new Date();
    const monthStart = new Date(now.getFullYear(), now.getMonth(), 1);
    const newThisMonth = patients.filter((p) => new Date(p.createdAt) >= monthStart).length;
    const activeCases = patients.filter((p) => p.status === 'Active').length;

    return {
      totalPatients: patients.length,
      newAdmissionsThisMonth: newThisMonth,
      activeCases,
      appointmentsToday: 12,
      weeklyActivity: [
        { day: 'Mon', patients: 45, appointments: 32 },
        { day: 'Tue', patients: 52, appointments: 38 },
        { day: 'Wed', patients: 48, appointments: 35 },
        { day: 'Thu', patients: 61, appointments: 42 },
        { day: 'Fri', patients: 55, appointments: 40 },
        { day: 'Sat', patients: 38, appointments: 28 },
        { day: 'Sun', patients: 28, appointments: 20 },
      ],
      departmentDistribution: [
        { name: 'Cardiology', value: 28 },
        { name: 'Neurology', value: 19 },
        { name: 'Orthopedics', value: 24 },
        { name: 'Pediatrics', value: 31 },
        { name: 'General', value: 25 },
      ],
      recentPatients: patients.slice(0, 5),
    };
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map((part) => part[0])
      .join('')
      .slice(0, 2)
      .toUpperCase();
  }

  statusClass(status: string): string {
    if (status === 'Critical') return 'status-critical';
    if (status === 'Active') return 'status-active';
    return 'status-default';
  }

  private applyStats(data: DashboardStats): void {
    this.stats.set([
      {
        title: 'Total Patients',
        value: data.totalPatients.toLocaleString(),
        change: '+12.5%',
        trend: 'up',
        icon: 'groups',
        colorClass: 'stat-blue',
        route: '/patients',
      },
      {
        title: 'New Admissions',
        value: data.newAdmissionsThisMonth.toString(),
        change: '+8.2%',
        trend: 'up',
        icon: 'person_add',
        colorClass: 'stat-green',
        route: '/patients/register',
      },
      {
        title: 'Appointments',
        value: data.appointmentsToday.toString(),
        change: '-3.1%',
        trend: 'down',
        icon: 'event',
        colorClass: 'stat-purple',
        route: '/appointments',
      },
      {
        title: 'Active Cases',
        value: data.activeCases.toString(),
        change: '+5.4%',
        trend: 'up',
        icon: 'monitor_heart',
        colorClass: 'stat-orange',
        route: '/patients',
      },
    ]);

    this.weeklyData.set(data.weeklyActivity);
    const maxVal = Math.max(
      ...data.weeklyActivity.flatMap((d) => [d.patients, d.appointments]),
      1,
    );
    this.maxWeeklyValue.set(maxVal);

    this.departmentData.set(
      data.departmentDistribution.map((item, index) => ({
        ...item,
        color: this.chartColors[index % this.chartColors.length],
      })),
    );

    this.recentPatients.set(data.recentPatients);
    this.upcomingAppointments.set(this.buildMockAppointments());
  }

  private buildMockAppointments(): AppointmentPreview[] {
    return [
      {
        patientName: 'Sarah Johnson',
        doctorName: 'Dr. Michael Chen',
        department: 'Cardiology',
        time: '09:00 AM',
        date: 'Today',
        type: 'Follow-up',
      },
      {
        patientName: 'Robert Williams',
        doctorName: 'Dr. Emily Davis',
        department: 'Neurology',
        time: '10:30 AM',
        date: 'Today',
        type: 'Consultation',
      },
      {
        patientName: 'Maria Garcia',
        doctorName: 'Dr. James Wilson',
        department: 'Pediatrics',
        time: '02:00 PM',
        date: 'Today',
        type: 'Check-up',
      },
      {
        patientName: 'David Brown',
        doctorName: 'Dr. Lisa Anderson',
        department: 'Orthopedics',
        time: '03:30 PM',
        date: 'Today',
        type: 'Surgery Prep',
      },
    ];
  }
}
