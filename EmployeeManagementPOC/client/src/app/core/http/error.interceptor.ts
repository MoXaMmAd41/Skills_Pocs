import { HttpErrorResponse, HttpInterceptorFn, HttpStatusCode } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { AuthService } from '../auth/auth.service';
import { NotificationService } from '../notifications/notification.service';
import { SKIP_ERROR_TOAST } from './api';
import { describeHttpError, toProblemDetails } from './problem-details';

/**
 * Central HTTP error policy:
 *  401 → session is gone, send the user to login and come back afterwards
 *  403 → access denied page
 *  everything else → toast with the server's problem-details message
 * The error is always re-thrown so callers can still react (e.g. map field errors onto a form).
 */
export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const notifications = inject(NotificationService);

  return next(request).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse)) {
        return throwError(() => error);
      }

      const skipToast = request.context.get(SKIP_ERROR_TOAST);

      switch (error.status) {
        case HttpStatusCode.Unauthorized:
          if (!skipToast) {
            auth.logout(router.url);
            notifications.warning('Your session has expired. Please sign in again.');
          }
          break;

        case HttpStatusCode.Forbidden:
          void router.navigate(['/access-denied']);
          break;

        default:
          if (!skipToast) {
            const { traceId } = toProblemDetails(error);
            notifications.error(describeHttpError(error), traceId);
          }
      }

      return throwError(() => error);
    }),
  );
};
