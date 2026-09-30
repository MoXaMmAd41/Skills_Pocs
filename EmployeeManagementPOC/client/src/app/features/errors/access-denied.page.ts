import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-access-denied-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink],
  template: `
    <div class="container py-5 text-center">
      <h1 class="display-5">Access Denied</h1>
      <p class="text-muted mt-3">You don't have permission to access this resource.</p>
      <a routerLink="/dashboard" class="btn btn-primary mt-3">Back to Dashboard</a>
    </div>
  `,
})
export class AccessDeniedPage {}
