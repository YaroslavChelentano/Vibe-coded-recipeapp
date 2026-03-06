import { apiFetch } from './client';

export interface AuthResponse {
  token: string;
  userId: number;
  email: string;
  displayName: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  displayName: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export function register(data: RegisterRequest) {
  return apiFetch<AuthResponse>('/auth/register', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export function login(data: LoginRequest) {
  return apiFetch<AuthResponse>('/auth/login', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}
