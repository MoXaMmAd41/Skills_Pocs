import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { AbstractControl, ValidationErrors } from '@angular/forms';

const MESSAGES: Record<string, (error: any) => string> = {
  required: () => 'This field is required.',
  email: () => 'Enter a valid email address.',
  maxlength: (e) => `Must be at most ${e.requiredLength} characters.`,
  min: (e) => `Must be at least ${e.min}.`,
  max: (e) => `Must be at most ${e.max}.`,
  pattern: () => 'The format is not valid.',
  server: (message: string) => message,
};

/**
 * Renders the first validation message of a control once the user has interacted with it.
 * Default change detection on purpose: control state isn't a signal, so it is re-read
 * whenever the host form's view is checked (input/blur events, submit).
 */
@Component({
  selector: 'app-field-error',
  changeDetection: ChangeDetectionStrategy.Default,
  template: `
    @if (message; as text) {
      <span class="field-validation-error">{{ text }}</span>
    }
  `,
})
export class FieldError {
  readonly control = input.required<AbstractControl | null>();

  protected get message(): string | null {
    const control = this.control();

    if (!control || !control.invalid || !(control.touched || control.dirty)) {
      return null;
    }

    return firstMessage(control.errors);
  }
}

function firstMessage(errors: ValidationErrors | null): string | null {
  if (!errors) {
    return null;
  }

  const [key, value] = Object.entries(errors)[0];
  return MESSAGES[key]?.(value) ?? 'This field is not valid.';
}
