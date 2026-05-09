export type UserRole = 'Regular' | 'Vip' | 'Organizer';

export interface AuthenticatedUser {
  id: string;
  email: string;
  displayName: string;
  role: UserRole;
}

export interface AuthResult {
  token: string;
  expiresAtUtc: string;
  user: AuthenticatedUser;
}

export interface RegisterInput {
  email: string;
  displayName: string;
  password: string;
}

export interface LoginInput {
  email: string;
  password: string;
}
