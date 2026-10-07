import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { Role } from './auth.models';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);

  return auth.accessToken
    ? true
    : inject(Router).createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};

export const guestGuard: CanActivateFn = () =>
  inject(AuthService).isAuthenticated() ? inject(Router).createUrlTree(['/']) : true;

/**
 * Restricts a route to the given roles. UX only — the API enforces the same policies.
 */
export function roleGuard(...roles: Role[]): CanActivateFn {
  return () =>
    inject(AuthService).hasAnyRole(roles) ? true : inject(Router).createUrlTree(['/access-denied']);
}
