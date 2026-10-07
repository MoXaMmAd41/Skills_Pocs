import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { apiUrl, SKIP_ERROR_TOAST, SKIP_LOADING } from '../../core/http/api';
import { Employee, EmployeeFilter, EmployeeRequest, EmployeeSuggestion, PagedResult } from './employee.models';

@Injectable({ providedIn: 'root' })
export class EmployeesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = apiUrl('v1', 'employees');

  search(filter: EmployeeFilter): Observable<PagedResult<Employee>> {
    return this.http.get<PagedResult<Employee>>(this.baseUrl, { params: toParams(filter) });
  }

  /** Search-as-you-type. Runs on keystrokes, so it stays silent: no progress bar, no error toasts. */
  suggest(prefix: string, limit = 8): Observable<EmployeeSuggestion[]> {
    return this.http.get<EmployeeSuggestion[]>(`${this.baseUrl}/suggestions`, {
      params: { prefix, limit },
      context: new HttpContext().set(SKIP_LOADING, true).set(SKIP_ERROR_TOAST, true),
    });
  }

  getById(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.baseUrl}/${id}`);
  }

  /** Validation failures are rendered inline by the form, so they skip the global toast. */
  create(request: EmployeeRequest): Observable<Employee> {
    return this.http.post<Employee>(this.baseUrl, request, { context: formContext() });
  }

  update(id: number, request: EmployeeRequest): Observable<Employee> {
    return this.http.put<Employee>(`${this.baseUrl}/${id}`, request, { context: formContext() });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}

function formContext(): HttpContext {
  return new HttpContext().set(SKIP_ERROR_TOAST, true);
}

function toParams(filter: EmployeeFilter): HttpParams {
  let params = new HttpParams()
    .set('sortBy', filter.sortBy)
    .set('sortDescending', filter.sortDescending)
    .set('page', filter.page)
    .set('pageSize', filter.pageSize);

  if (filter.search.trim()) {
    params = params.set('search', filter.search.trim());
  }

  if (filter.departmentId !== null) {
    params = params.set('departmentId', filter.departmentId);
  }

  if (filter.isActive !== null) {
    params = params.set('isActive', filter.isActive);
  }

  return params;
}
