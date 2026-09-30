export type Role = 'Admin' | 'HR' | 'Manager' | 'Employee';

export const MANAGE_ROLES: readonly Role[] = ['Admin', 'HR'];
export const DEPARTMENT_VIEW_ROLES: readonly Role[] = ['Admin', 'HR', 'Manager'];

export interface CurrentUser {
  id: string;
  email: string;
  fullName: string;
  roles: Role[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  expiresAt: string;
  user: CurrentUser;
}
