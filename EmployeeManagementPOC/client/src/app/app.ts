import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { LoadingService } from './core/loading/loading.service';
import { ToastContainer } from './core/notifications/toast-container';

@Component({
  selector: 'app-root',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, ToastContainer],
  template: `
    @if (loading.isLoading()) {
      <div class="global-progress" role="progressbar" aria-label="Loading"></div>
    }
    <router-outlet />
    <app-toast-container />
  `,
})
export class App {
  protected readonly loading = inject(LoadingService);
}
