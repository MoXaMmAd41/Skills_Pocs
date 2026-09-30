import { Routes } from '@angular/router';

import { roleGuard } from '../../core/auth/auth.guards';

export const DEPARTMENT_ROUTES: Routes = [
  {
    path: '',
    title: 'Departments - Employee Management',
    loadComponent: () => import('./department-list.page').then((m) => m.DepartmentListPage),
  },
  {
    path: 'new',
    title: 'Create Department - Employee Management',
    canActivate: [roleGuard('Admin', 'HR')],
    loadComponent: () => import('./department-form.page').then((m) => m.DepartmentFormPage),
  },
  {
    path: ':id/edit',
    title: 'Edit Department - Employee Management',
    canActivate: [roleGuard('Admin', 'HR')],
    loadComponent: () => import('./department-form.page').then((m) => m.DepartmentFormPage),
  },
];
