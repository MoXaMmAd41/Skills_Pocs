export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string | null;
  salary: number;
  hireDate: string;
  isActive: boolean;
  departmentId: number;
  departmentName: string;
}

export interface EmployeeSuggestion {
  id: number;
  fullName: string;
  email: string;
  departmentName: string;
}

export interface EmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string | null;
  salary: number;
  hireDate: string;
  isActive: boolean;
  departmentId: number;
}

export type EmployeeSortField = 'Name' | 'Salary' | 'HireDate';

export interface EmployeeFilter {
  search: string;
  departmentId: number | null;
  isActive: boolean | null;
  sortBy: EmployeeSortField;
  sortDescending: boolean;
  page: number;
  pageSize: number;
}

export const DEFAULT_EMPLOYEE_FILTER: EmployeeFilter = {
  search: '',
  departmentId: null,
  isActive: null,
  sortBy: 'Name',
  sortDescending: false,
  page: 1,
  pageSize: 10,
};

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
