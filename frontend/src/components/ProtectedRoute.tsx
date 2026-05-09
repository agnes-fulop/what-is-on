import { Navigate, useLocation } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import type { UserRole } from '../types';

interface ProtectedRouteProps {
  children: React.ReactNode;
  role?: UserRole | UserRole[];
}

/**
 * Redirects to /login if the user isn't authenticated, or to / if they're
 * authenticated but lack the required role. Currently unused by Phase 8's
 * routes (the API enforces authorization) but kept here as the canonical
 * client-side gate for future protected views.
 */
export function ProtectedRoute({ children, role }: ProtectedRouteProps) {
  const location = useLocation();
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated());
  const hasRole = useAuthStore((s) => (role ? s.hasRole(role) : true));

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (!hasRole) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}
