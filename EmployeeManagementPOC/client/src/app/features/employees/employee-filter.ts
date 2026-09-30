import { ParamMap, Params } from '@angular/router';

import { DEFAULT_EMPLOYEE_FILTER, EmployeeFilter, EmployeeSortField } from './employee.models';

const SORT_FIELDS: readonly EmployeeSortField[] = ['Name', 'Salary', 'HireDate'];

/**
 * The URL is the source of truth for list state, so filters survive refresh,
 * back/forward navigation and can be shared as links.
 */
export function filterFromQuery(params: ParamMap): EmployeeFilter {
  const defaults = DEFAULT_EMPLOYEE_FILTER;
  const sortBy = params.get('sortBy') as EmployeeSortField | null;

  return {
    search: params.get('search') ?? defaults.search,
    departmentId: toPositiveInt(params.get('departmentId')),
    isActive: toBoolean(params.get('isActive')),
    sortBy: sortBy && SORT_FIELDS.includes(sortBy) ? sortBy : defaults.sortBy,
    sortDescending: params.get('sortDescending') === 'true',
    page: toPositiveInt(params.get('page')) ?? defaults.page,
    pageSize: defaults.pageSize,
  };
}

/** Only non-default values are written, keeping URLs short. */
export function filterToQuery(filter: EmployeeFilter): Params {
  const defaults = DEFAULT_EMPLOYEE_FILTER;

  return {
    search: filter.search.trim() || null,
    departmentId: filter.departmentId,
    isActive: filter.isActive,
    sortBy: filter.sortBy === defaults.sortBy ? null : filter.sortBy,
    sortDescending: filter.sortDescending || null,
    page: filter.page === defaults.page ? null : filter.page,
  };
}

function toPositiveInt(value: string | null): number | null {
  const parsed = Number(value);
  return Number.isInteger(parsed) && parsed > 0 ? parsed : null;
}

function toBoolean(value: string | null): boolean | null {
  return value === 'true' ? true : value === 'false' ? false : null;
}
