import { ChangeDetectionStrategy, Component, inject } from '@angular/core';

import { NotificationService } from './notification.service';

@Component({
  selector: 'app-toast-container',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="toast-stack" aria-live="polite">
      @for (toast of notifications.notifications(); track toast.id) {
        <div [class]="'app-toast ' + toast.kind" role="alert">
          <div>
            {{ toast.message }}
            @if (toast.traceId) {
              <small class="app-toast-trace">Reference: {{ toast.traceId }}</small>
            }
          </div>
          <button type="button" aria-label="Dismiss" (click)="notifications.dismiss(toast.id)">×</button>
        </div>
      }
    </div>
  `,
})
export class ToastContainer {
  protected readonly notifications = inject(NotificationService);
}
