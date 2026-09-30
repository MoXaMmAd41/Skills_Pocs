import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink],
  template: `
    <div class="container py-5 text-center">
      <h1 class="display-5">Page not found</h1>
      <p class="text-muted mt-3">The page you are looking for doesn't exist or has been moved.</p>
      <a routerLink="/dashboard" class="btn btn-primary mt-3">Back to Dashboard</a>
    </div>
  `,
})
export class NotFoundPage {}
