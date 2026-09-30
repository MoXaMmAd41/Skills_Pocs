import { HttpErrorResponse } from '@angular/common/http';
import { AbstractControl, FormGroup } from '@angular/forms';

import { describeHttpError, toProblemDetails } from '../../core/http/problem-details';

/**
 * Maps a failed save onto the form:
 *  - 400 validation problems → a `server` error on each matching control
 *  - known business error codes (e.g. `Employee.DuplicateEmail`) → the control named in `fieldByCode`
 * Returns a message for anything that could not be attached to a field, or null if everything was.
 */
export function applyServerErrors(
  form: FormGroup,
  error: unknown,
  fieldByCode: Record<string, string> = {},
): string | null {
  if (!(error instanceof HttpErrorResponse)) {
    return 'An unexpected error occurred.';
  }

  const problem = toProblemDetails(error);
  const codeField = problem.code ? form.get(fieldByCode[problem.code] ?? '') : null;

  if (codeField && problem.detail) {
    setServerError(codeField, problem.detail);
    return null;
  }

  if (!problem.errors) {
    return describeHttpError(error);
  }

  const unmatched: string[] = [];

  for (const [field, messages] of Object.entries(problem.errors)) {
    const control = form.get(field);

    if (control) {
      setServerError(control, messages[0]);
    } else {
      unmatched.push(...messages);
    }
  }

  return unmatched.length ? unmatched.join(' ') : null;
}

function setServerError(control: AbstractControl, message: string): void {
  control.setErrors({ ...control.errors, server: message });
  control.markAsTouched();
}
