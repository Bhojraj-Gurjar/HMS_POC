import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../../../core/services/auth.service';
import { LoadingService } from '../../../core/services/loading.service';
import { NotificationService } from '../../../core/services/notification.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly loading = inject(LoadingService);
  private readonly notifications = inject(NotificationService);

  readonly form = this.fb.nonNullable.group({
    username: ['admin', Validators.required],
    password: ['admin123', Validators.required],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.error('Please enter both username and password');
      return;
    }

    this.loading.show();
    this.auth
      .login(this.form.getRawValue())
      .pipe(finalize(() => this.loading.hide()))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notifications.success('Login successful!');
            void this.router.navigate(['/dashboard']);
          }
        },
      });
  }
}
