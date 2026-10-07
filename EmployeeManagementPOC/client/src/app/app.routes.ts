import { Routes } from '@angular/router';

import { authGuard, guestGuard, roleGuard } from './core/auth/auth.guards';
import { Shell } from './core/layout/shell';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    title: 'Sign In - Employee Management',
    loadComponent: () => import('./features/auth/login.page').then((m) => m.LoginPage),
  },
  {
    path: '',
    component: Shell,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        title: 'Dashboard - Employee Management',
        loadComponent: () => import('./features/dashboard/dashboard.page').then((m) => m.DashboardPage),
      },
      {
        path: 'employees',
        loadChildren: () => import('./features/employees/employees.routes').then((m) => m.EMPLOYEE_ROUTES),
      },
      {
        path: 'departments',
        canActivate: [roleGuard('Admin', 'HR', 'Manager')],
        loadChildren: () => import('./features/departments/departments.routes').then((m) => m.DEPARTMENT_ROUTES),
      },
      {
        path: 'access-denied',
        title: 'Access Denied - Employee Management',
        loadComponent: () => import('./features/errors/access-denied.page').then((m) => m.AccessDeniedPage),
      },
      {
        path: '**',
        title: 'Not Found - Employee Management',
        loadComponent: () => import('./features/errors/not-found.page').then((m) => m.NotFoundPage),
      },
    ],
  },
];
