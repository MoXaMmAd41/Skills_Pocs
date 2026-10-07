import { HttpErrorResponse } from '@angular/common/http';

/** RFC 9457 problem details, as emitted by the API's global exception handler. */
export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  code?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}

export function toProblemDetails(error: HttpErrorResponse): ProblemDetails {
  const body = error.error;

  if (body && typeof body === 'object' && ('title' in body || 'detail' in body)) {
    return body as ProblemDetails;
  }

  return { status: error.status, title: error.statusText };
}

/** Human-readable message for a failed request, preferring the server's own explanation. */
export function describeHttpError(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'Cannot reach the server. Check your connection and try again.';
  }

  const problem = toProblemDetails(error);

  if (problem.errors) {
    return Object.values(problem.errors).flat()[0] ?? 'The request is invalid.';
  }

  if (error.status >= 500) {
    return 'An unexpected server error occurred.';
  }

  return problem.detail ?? problem.title ?? 'An unexpected error occurred.';
}
