import { Pipe, PipeTransform } from '@angular/core';

/** `1 | pluralize: 'employee'` → `1 employee`; `3 | pluralize: 'employee'` → `3 employees`. */
@Pipe({ name: 'pluralize' })
export class PluralizePipe implements PipeTransform {
  transform(count: number, singular: string, plural = `${singular}s`): string {
    return `${count} ${count === 1 ? singular : plural}`;
  }
}
