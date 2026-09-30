import { Pipe, PipeTransform } from '@angular/core';

/** `'Jane Doe' | initials` → `J`; `'Jane Doe' | initials: 2` → `JD`. */
@Pipe({ name: 'initials' })
export class InitialsPipe implements PipeTransform {
  transform(value: string | null | undefined, max = 1): string {
    const initials = (value ?? '')
      .split(/\s+/)
      .filter(Boolean)
      .slice(0, max)
      .map((word) => word[0].toUpperCase())
      .join('');

    return initials || '?';
  }
}
