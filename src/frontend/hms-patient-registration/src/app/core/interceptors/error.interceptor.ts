import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';
import { LoadingService } from '../services/loading.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notifications = inject(NotificationService);
  const loading = inject(LoadingService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      loading.hide();
      const message =
        error.error?.message ??
        error.error?.errors?.join(', ') ??
        error.message ??
        'An unexpected error occurred.';
      notifications.error(message);
      return throwError(() => error);
    }),
  );
};
