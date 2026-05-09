import { useActionState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { ApiError } from '../api/client';
import { registrationsApi } from '../api';
import { ComponentRenderer } from '../components/layout/ComponentRenderer';
import { useEventDetail } from '../hooks/useEventDetail';
import { useAuthStore } from '../store/authStore';
import type { EventDetail } from '../types';
import { formatDate, formatPrice } from '../utils/format';

type RegistrationState =
  | { kind: 'idle' }
  | { kind: 'success' }
  | { kind: 'error'; message: string };

export function EventDetailPage() {
  const { id = '' } = useParams<{ id: string }>();
  const { event, loading, error } = useEventDetail(id);

  if (loading) {
    return <div className="page container center">Loading event…</div>;
  }

  if (error) {
    return (
      <div className="page container">
        <ErrorState error={error} />
      </div>
    );
  }

  if (!event) {
    return null;
  }

  return (
    <main className="page">
      <div className="container">
        <EventHero event={event} />
        <EventLayout event={event} />
      </div>
    </main>
  );
}

function ErrorState({ error }: { error: Error }) {
  if (error instanceof ApiError) {
    if (error.status === 401) {
      return (
        <div className="alert alert--error">
          This event is restricted. <Link to="/login">Log in</Link> to view it.
        </div>
      );
    }
    if (error.status === 403) {
      return (
        <div className="alert alert--error">
          This event is restricted to VIP members.
        </div>
      );
    }
    if (error.status === 404) {
      return <div className="alert alert--error">Event not found.</div>;
    }
  }
  return <div className="alert alert--error">Could not load event: {error.message}</div>;
}

function EventHero({ event }: { event: EventDetail }) {
  return (
    <section className="event-hero">
      {event.isVip && <span className="badge badge--vip">VIP</span>}
      <h1>{event.title}</h1>
      {event.subtitle && <p className="event-hero__subtitle">{event.subtitle}</p>}
      <div className="event-hero__meta">
        <span>{formatDate(event.date)}</span>
        <span>
          {event.location.venue}, {event.location.city}
        </span>
        <span>by {event.organizer.name}</span>
      </div>
      <div className="event-hero__cta">
        <RegisterControl event={event} />
      </div>
    </section>
  );
}

function RegisterControl({ event }: { event: EventDetail }) {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated());

  const [regState, registerAction, registerPending] = useActionState<RegistrationState, FormData>(
    async () => {
      try {
        await registrationsApi.registerForEvent(event.id);
        return { kind: 'success' };
      } catch (err) {
        const message = err instanceof ApiError ? err.detail || err.title : 'Registration failed';
        return { kind: 'error', message };
      }
    },
    { kind: 'idle' },
  );

  const today = new Date().toISOString().slice(0, 10);
  const opensInFuture = today < event.registration.openDate;
  const closed = today > event.registration.closeDate;

  if (regState.kind === 'success') {
    return <div className="alert alert--success">You're registered for this event.</div>;
  }

  if (opensInFuture) {
    return (
      <p>
        Registration opens on {formatDate(event.registration.openDate)}.
      </p>
    );
  }

  if (closed) {
    return (
      <p>
        Registration closed on {formatDate(event.registration.closeDate)}.
      </p>
    );
  }

  if (!isAuthenticated) {
    return (
      <Link to="/login" className="btn btn--primary">
        Log in to register
      </Link>
    );
  }

  return (
    <form action={registerAction}>
      <button type="submit" className="btn btn--primary" disabled={registerPending}>
        {registerPending ? 'Registering…' : `${event.hero.ctaText || 'Register'} · ${formatPrice(event.registration.fee)}`}
      </button>
      {regState.kind === 'error' && (
        <p className="alert alert--error" style={{ marginTop: '0.75rem' }}>
          {regState.message}
        </p>
      )}
    </form>
  );
}

function EventLayout({ event }: { event: EventDetail }) {
  if (event.layout && event.layout.components.length > 0) {
    return (
      <div>
        {event.layout.components.map((component) => (
          <ComponentRenderer key={component.id} component={component} />
        ))}
      </div>
    );
  }

  // Fallback for events without a layout: render the description and a basic
  // session list so the page is still useful.
  return (
    <div>
      {event.description && <p>{event.description}</p>}
      {event.sessions.length > 0 && (
        <section className="session-schedule">
          <h3>Schedule</h3>
          <div className="session-schedule__cards">
            {event.sessions.map((session) => (
              <div key={session.id} className="session-card">
                <div className="session-card__time">
                  {new Date(session.from).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </div>
                <div>
                  <p className="session-card__title">{session.title}</p>
                  <p className="session-card__meta">
                    {session.room}
                    {session.speaker ? ` · ${session.speaker.name}` : ''}
                  </p>
                </div>
              </div>
            ))}
          </div>
        </section>
      )}
    </div>
  );
}
