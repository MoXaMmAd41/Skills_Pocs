import { Injectable, signal } from '@angular/core';

export type NotificationKind = 'success' | 'error' | 'warning';

export interface Notification {
  id: number;
  kind: NotificationKind;
  message: string;
  /** Correlates the toast with server logs when reporting a problem. */
  traceId?: string;
}

const AUTO_DISMISS_MS = 5000;

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private nextId = 0;
  private readonly items = signal<Notification[]>([]);

  readonly notifications = this.items.asReadonly();

  success(message: string): void {
    this.push('success', message);
  }

  warning(message: string): void {
    this.push('warning', message);
  }

  error(message: string, traceId?: string): void {
    this.push('error', message, traceId);
  }

  dismiss(id: number): void {
    this.items.update((items) => items.filter((item) => item.id !== id));
  }

  private push(kind: NotificationKind, message: string, traceId?: string): void {
    // Collapse bursts of the same failure (e.g. several parallel requests hitting a down server).
    if (this.items().some((item) => item.kind === kind && item.message === message)) {
      return;
    }

    const id = ++this.nextId;
    this.items.update((items) => [...items, { id, kind, message, traceId }]);
    setTimeout(() => this.dismiss(id), AUTO_DISMISS_MS);
  }
}
