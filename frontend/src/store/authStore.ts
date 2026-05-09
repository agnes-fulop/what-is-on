import { create } from 'zustand';
import type { AuthResult, AuthenticatedUser, UserRole } from '../types';

const STORAGE_KEY = 'whatison.auth';

interface AuthState {
  token: string | null;
  user: AuthenticatedUser | null;
  expiresAtUtc: string | null;

  setSession: (result: AuthResult) => void;
  clearSession: () => void;
  isAuthenticated: () => boolean;
  hasRole: (role: UserRole | UserRole[]) => boolean;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  ...loadInitialState(),

  setSession: (result) => {
    persist(result);
    set({
      token: result.token,
      user: result.user,
      expiresAtUtc: result.expiresAtUtc,
    });
  },

  clearSession: () => {
    localStorage.removeItem(STORAGE_KEY);
    set({ token: null, user: null, expiresAtUtc: null });
  },

  isAuthenticated: () => {
    const { token, expiresAtUtc } = get();
    return token !== null && !isExpired(expiresAtUtc);
  },

  hasRole: (role) => {
    const userRole = get().user?.role;
    if (!userRole) return false;
    return Array.isArray(role) ? role.includes(userRole) : userRole === role;
  },
}));

interface PersistedAuth {
  token: string;
  user: AuthenticatedUser;
  expiresAtUtc: string;
}

function loadInitialState(): Pick<AuthState, 'token' | 'user' | 'expiresAtUtc'> {
  const empty = { token: null, user: null, expiresAtUtc: null };

  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return empty;

    const parsed = JSON.parse(raw) as PersistedAuth;
    if (isExpired(parsed.expiresAtUtc)) {
      localStorage.removeItem(STORAGE_KEY);
      return empty;
    }

    return {
      token: parsed.token,
      user: parsed.user,
      expiresAtUtc: parsed.expiresAtUtc,
    };
  } catch {
    localStorage.removeItem(STORAGE_KEY);
    return empty;
  }
}

function persist(result: AuthResult) {
  const payload: PersistedAuth = {
    token: result.token,
    user: result.user,
    expiresAtUtc: result.expiresAtUtc,
  };
  localStorage.setItem(STORAGE_KEY, JSON.stringify(payload));
}

function isExpired(expiresAtUtc: string | null): boolean {
  if (!expiresAtUtc) return true;
  const expiry = new Date(expiresAtUtc).getTime();
  return Number.isNaN(expiry) || expiry <= Date.now();
}
