import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';

import { InitialsPipe } from '../../shared/pipes/initials.pipe';
import { PluralizePipe } from '../../shared/pipes/pluralize.pipe';
import { DashboardService } from './dashboard.service';

@Component({
  selector: 'app-dashboard-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, DecimalPipe, CurrencyPipe, InitialsPipe, PluralizePipe],
  templateUrl: './dashboard.page.html',
})
export class DashboardPage {
  // Failures are already surfaced by the error interceptor; render the empty state.
  protected readonly dashboard = toSignal(inject(DashboardService).get().pipe(catchError(() => of(null))));
  protected readonly today = new Date();
}
