import { HttpClient, HttpContext } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

import { apiUrl, SKIP_ERROR_TOAST } from '../http/api';
import { AuthResponse, LoginRequest, Role } from './auth.models';

const STORAGE_KEY = 'em.session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly session = signal<AuthResponse | null>(readStoredSession());

  readonly user = computed(() => this.session()?.user ?? null);
  readonly isAuthenticated = computed(() => this.session() !== null);

  get accessToken(): string | null {
    const session = this.session();

    if (session && isExpired(session)) {
      this.clearSession();
      return null;
    }

    return session?.accessToken ?? null;
  }

  login(request: LoginRequest, remember: boolean): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(apiUrl('v1', 'auth', 'login'), request, {
        // The login form shows the failure inline.
        context: new HttpContext().set(SKIP_ERROR_TOAST, true),
      })
      .pipe(tap((response) => this.storeSession(response, remember)));
  }

  logout(returnUrl?: string): void {
    this.clearSession();
    void this.router.navigate(['/login'], { queryParams: returnUrl ? { returnUrl } : {} });
  }

  hasAnyRole(roles: readonly Role[]): boolean {
    const userRoles = this.user()?.roles ?? [];
    return roles.some((role) => userRoles.includes(role));
  }

  private storeSession(response: AuthResponse, remember: boolean): void {
    this.session.set(response);
    (remember ? localStorage : sessionStorage).setItem(STORAGE_KEY, JSON.stringify(response));
  }

  private clearSession(): void {
    this.session.set(null);
    localStorage.removeItem(STORAGE_KEY);
    sessionStorage.removeItem(STORAGE_KEY);
  }
}

function readStoredSession(): AuthResponse | null {
  try {
    const raw = sessionStorage.getItem(STORAGE_KEY) ?? localStorage.getItem(STORAGE_KEY);
    const session = raw ? (JSON.parse(raw) as AuthResponse) : null;
    return session && !isExpired(session) ? session : null;
  } catch {
    return null;
  }
}

function isExpired(session: AuthResponse): boolean {
  return new Date(session.expiresAt).getTime() <= Date.now();
}
