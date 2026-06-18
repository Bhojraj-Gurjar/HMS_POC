import { HttpInterceptorFn } from '@angular/common/http';

const CORRELATION_HEADER = 'X-Correlation-Id';

export const correlationIdInterceptor: HttpInterceptorFn = (req, next) => {
  const correlationId = crypto.randomUUID();
  return next(
    req.clone({
      setHeaders: { [CORRELATION_HEADER]: correlationId },
    }),
  );
};
