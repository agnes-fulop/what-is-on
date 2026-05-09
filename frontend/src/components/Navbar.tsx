import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';

export function Navbar() {
  const navigate = useNavigate();
  const user = useAuthStore((s) => s.user);
  const clearSession = useAuthStore((s) => s.clearSession);

  const handleLogout = () => {
    clearSession();
    navigate('/');
  };

  return (
    <nav className="nav">
      <div className="container nav__inner">
        <Link to="/" className="nav__brand">
          What's On
        </Link>
        <div className="nav__actions">
          {user ? (
            <>
              <span className="nav__user">
                {user.displayName} <span className="muted">· {user.role}</span>
              </span>
              <button className="btn btn--ghost" onClick={handleLogout}>
                Log out
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="btn btn--ghost">
                Log in
              </Link>
              <Link to="/register" className="btn btn--primary">
                Sign up
              </Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
