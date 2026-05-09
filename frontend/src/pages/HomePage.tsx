import { EventCard } from '../components/events/EventCard';
import { useEvents } from '../hooks/useEvents';

export function HomePage() {
  const { events, loading, error } = useEvents();

  return (
    <main className="page">
      <div className="container">
        <h1>Events</h1>
        <p className="muted">Browse upcoming and past events.</p>

        {loading && <div className="center">Loading events…</div>}

        {error && (
          <div className="alert alert--error">
            Could not load events: {error.message}
          </div>
        )}

        {!loading && !error && events.length === 0 && (
          <div className="center">No events yet.</div>
        )}

        {events.length > 0 && (
          <div className="event-grid">
            {events.map((event) => (
              <EventCard key={event.id} event={event} />
            ))}
          </div>
        )}
      </div>
    </main>
  );
}
