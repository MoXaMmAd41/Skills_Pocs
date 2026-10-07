import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { apiUrl } from '../../core/http/api';

export interface DepartmentDistribution {
  departmentId: number;
  departmentName: string;
  employeeCount: number;
  /** Headcount relative to the largest department, 0–100. */
  relativeSize: number;
}

export interface Dashboard {
  totalEmployees: number;
  activeEmployees: number;
  inactiveEmployees: number;
  totalDepartments: number;
  averageSalary: number;
  employeesByDepartment: DepartmentDistribution[];
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);

  /** Uses v2: v1 is deprecated and lacks the server-computed fields. */
  get(): Observable<Dashboard> {
    return this.http.get<Dashboard>(apiUrl('v2', 'dashboard'));
  }
}
