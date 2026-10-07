import { Pipe, PipeTransform } from '@angular/core';

/**
 * Case-insensitive client-side filter over the given fields.
 * `departments | filterBy: term : ['name', 'description']`
 *
 * Pure, so it only re-runs when the array reference, term or keys change.
 * Use for small, fully-loaded lists; large lists should filter server-side.
 */
@Pipe({ name: 'filterBy' })
export class FilterByPipe implements PipeTransform {
  transform<T>(items: readonly T[] | null | undefined, term: string | null | undefined, keys: (keyof T)[]): T[] {
    const list = [...(items ?? [])];
    const needle = term?.trim().toLowerCase();

    if (!needle) {
      return list;
    }

    return list.filter((item) =>
      keys.some((key) => String(item[key] ?? '').toLowerCase().includes(needle)),
    );
  }
}
