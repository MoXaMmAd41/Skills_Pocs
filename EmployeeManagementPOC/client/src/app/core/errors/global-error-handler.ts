import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandler, inject, Injectable } from '@angular/core';

import { NotificationService } from '../notifications/notification.service';

/**
 * Last line of defence for errors thrown outside HTTP calls (template bugs, rejected promises).
 * HTTP failures are already reported by the error interceptor, so they are only logged here.
 */
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private readonly notifications = inject(NotificationService);

  handleError(error: unknown): void {
    console.error(error);

    if (!(error instanceof HttpErrorResponse)) {
      this.notifications.error('Something went wrong. Please refresh the page and try again.');
    }
  }
}
