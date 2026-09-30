import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';
import { describeHttpError } from '../../core/http/problem-details';
import { FieldError } from '../../shared/forms/field-error';

@Component({
  selector: 'app-login-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, FieldError],
  templateUrl: './login.page.html',
  host: { class: 'auth-body d-block' },
})
export class LoginPage {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  /** Bound from the `?returnUrl=` query parameter. */
  readonly returnUrl = input<string>();

  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly form = inject(NonNullableFormBuilder).group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    rememberMe: [false],
  });

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, password, rememberMe } = this.form.getRawValue();

    this.submitting.set(true);
    this.error.set(null);

    this.auth.login({ email, password }, rememberMe).subscribe({
      next: () => void this.router.navigateByUrl(this.safeReturnUrl()),
      error: (error: HttpErrorResponse) => {
        this.error.set(describeHttpError(error));
        this.submitting.set(false);
      },
    });
  }

  // Only allow in-app paths to prevent open redirects.
  private safeReturnUrl(): string {
    const url = this.returnUrl();
    return url?.startsWith('/') && !url.startsWith('//') ? url : '/dashboard';
  }
}
