import { HttpClient, HttpContext } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { apiUrl, SKIP_ERROR_TOAST } from '../../core/http/api';

export interface Department {
  id: number;
  name: string;
  description: string | null;
  employeesCount: number;
}

export interface DepartmentRequest {
  name: string;
  description: string | null;
}

@Injectable({ providedIn: 'root' })
export class DepartmentsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = apiUrl('v1', 'departments');

  getAll(): Observable<Department[]> {
    return this.http.get<Department[]>(this.baseUrl);
  }

  getById(id: number): Observable<Department> {
    return this.http.get<Department>(`${this.baseUrl}/${id}`);
  }

  create(request: DepartmentRequest): Observable<Department> {
    return this.http.post<Department>(this.baseUrl, request, { context: formContext() });
  }

  update(id: number, request: DepartmentRequest): Observable<Department> {
    return this.http.put<Department>(`${this.baseUrl}/${id}`, request, { context: formContext() });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}

function formContext(): HttpContext {
  return new HttpContext().set(SKIP_ERROR_TOAST, true);
}
