import type { AuthResult, LoginInput, RegisterInput } from '../types';
import { apiFetch } from './client';

export function register(input: RegisterInput): Promise<AuthResult> {
  return apiFetch<AuthResult>('/api/auth/register', {
    method: 'POST',
    body: input,
  });
}

export function login(input: LoginInput): Promise<AuthResult> {
  return apiFetch<AuthResult>('/api/auth/login', {
    method: 'POST',
    body: input,
  });
}
