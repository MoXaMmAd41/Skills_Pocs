import { HttpContextToken } from '@angular/common/http';

/** Versions of the backend API this client speaks. Bump per feature, not globally. */
export type ApiVersion = 'v1' | 'v2';

/** Builds a versioned API URL, e.g. `apiUrl('v1', 'employees', 42)` → `/api/v1/employees/42`. */
export function apiUrl(version: ApiVersion, ...segments: (string | number)[]): string {
  return ['/api', version, ...segments].join('/');
}

export function isApiRequest(url: string): boolean {
  return url.startsWith('/api/');
}

/**
 * Opt-out flags a caller can set per request via `HttpContext`.
 * Use when the component renders the error itself (e.g. inline form errors).
 */
export const SKIP_ERROR_TOAST = new HttpContextToken<boolean>(() => false);

/** Background requests (e.g. debounced search) that should not show the global progress bar. */
export const SKIP_LOADING = new HttpContextToken<boolean>(() => false);
