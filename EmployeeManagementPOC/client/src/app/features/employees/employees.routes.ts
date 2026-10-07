import { Routes } from '@angular/router';

import { roleGuard } from '../../core/auth/auth.guards';

export const EMPLOYEE_ROUTES: Routes = [
  {
    path: '',
    title: 'Employees - Employee Management',
    loadComponent: () => import('./employee-list.page').then((m) => m.EmployeeListPage),
  },
  {
    path: 'new',
    title: 'Create Employee - Employee Management',
    canActivate: [roleGuard('Admin', 'HR')],
    loadComponent: () => import('./employee-form.page').then((m) => m.EmployeeFormPage),
  },
  {
    path: ':id',
    title: 'Employee Details - Employee Management',
    loadComponent: () => import('./employee-details.page').then((m) => m.EmployeeDetailsPage),
  },
  {
    path: ':id/edit',
    title: 'Edit Employee - Employee Management',
    canActivate: [roleGuard('Admin', 'HR')],
    loadComponent: () => import('./employee-form.page').then((m) => m.EmployeeFormPage),
  },
];
