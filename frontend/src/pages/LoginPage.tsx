import { useActionState, useEffect } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { authApi } from '../api';
import { ApiError } from '../api/client';
import { SubmitButton } from '../components/SubmitButton';
import { useAuthStore } from '../store/authStore';

interface LoginState {
  status: 'idle' | 'success' | 'error';
  error: string | null;
}

const initialState: LoginState = { status: 'idle', error: null };

export function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const setSession = useAuthStore((s) => s.setSession);
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated());

  const redirectTo = (location.state as { from?: { pathname: string } } | null)?.from?.pathname ?? '/';

  // Redirect already-authenticated users straight to the destination.
  useEffect(() => {
    if (isAuthenticated) {
      navigate(redirectTo, { replace: true });
    }
  }, [isAuthenticated, navigate, redirectTo]);

  const [state, formAction] = useActionState<LoginState, FormData>(async (_prev, formData) => {
    try {
      const result = await authApi.login({
        email: String(formData.get('email') ?? ''),
        password: String(formData.get('password') ?? ''),
      });
      setSession(result);
      return { status: 'success', error: null };
    } catch (err) {
      const message = err instanceof ApiError ? err.detail || err.title : 'Login failed';
      return { status: 'error', error: message };
    }
  }, initialState);

  // Navigate after successful login (effect, not inside the action).
  useEffect(() => {
    if (state.status === 'success') {
      navigate(redirectTo, { replace: true });
    }
  }, [state.status, navigate, redirectTo]);

  return (
    <main className="page">
      <form className="auth-form" action={formAction}>
        <h1>Log in</h1>

        {state.error && <div className="alert alert--error">{state.error}</div>}

        <div className="field">
          <label htmlFor="email">Email</label>
          <input id="email" name="email" type="email" autoComplete="email" required />
        </div>

        <div className="field">
          <label htmlFor="password">Password</label>
          <input id="password" name="password" type="password" autoComplete="current-password" required />
        </div>

        <SubmitButton idleLabel="Log in" pendingLabel="Logging in…" />

        <p className="muted" style={{ marginTop: '1rem' }}>
          New here? <Link to="/register">Create an account</Link>
        </p>
      </form>
    </main>
  );
}
