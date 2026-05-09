import { useActionState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authApi } from '../api';
import { ApiError } from '../api/client';
import { SubmitButton } from '../components/SubmitButton';
import { useAuthStore } from '../store/authStore';

interface RegisterState {
  status: 'idle' | 'success' | 'error';
  error: string | null;
}

const initialState: RegisterState = { status: 'idle', error: null };

export function RegisterPage() {
  const navigate = useNavigate();
  const setSession = useAuthStore((s) => s.setSession);
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated());

  useEffect(() => {
    if (isAuthenticated) navigate('/', { replace: true });
  }, [isAuthenticated, navigate]);

  const [state, formAction] = useActionState<RegisterState, FormData>(async (_prev, formData) => {
    try {
      const result = await authApi.register({
        email: String(formData.get('email') ?? ''),
        displayName: String(formData.get('displayName') ?? ''),
        password: String(formData.get('password') ?? ''),
      });
      setSession(result);
      return { status: 'success', error: null };
    } catch (err) {
      const message = err instanceof ApiError ? err.detail || err.title : 'Registration failed';
      return { status: 'error', error: message };
    }
  }, initialState);

  useEffect(() => {
    if (state.status === 'success') {
      navigate('/', { replace: true });
    }
  }, [state.status, navigate]);

  return (
    <main className="page">
      <form className="auth-form" action={formAction}>
        <h1>Create account</h1>

        {state.error && <div className="alert alert--error">{state.error}</div>}

        <div className="field">
          <label htmlFor="email">Email</label>
          <input id="email" name="email" type="email" autoComplete="email" required />
        </div>

        <div className="field">
          <label htmlFor="displayName">Display name</label>
          <input id="displayName" name="displayName" type="text" autoComplete="name" required />
        </div>

        <div className="field">
          <label htmlFor="password">Password</label>
          <input
            id="password"
            name="password"
            type="password"
            autoComplete="new-password"
            minLength={8}
            required
          />
          <small className="muted">At least 8 characters.</small>
        </div>

        <SubmitButton idleLabel="Create account" pendingLabel="Creating account…" />

        <p className="muted" style={{ marginTop: '1rem' }}>
          Already have an account? <Link to="/login">Log in</Link>
        </p>
      </form>
    </main>
  );
}
