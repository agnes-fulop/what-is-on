import { Link } from 'react-router-dom';
import type { EventSummary } from '../../types';
import { formatDate, formatPrice } from '../../utils/format';

export function EventCard({ event }: { event: EventSummary }) {
  return (
    <Link to={`/events/${event.id}`} className="event-card">
      <div className="event-card__hero">
        {event.hero.image && <img src={event.hero.image} alt="" />}
        {event.isVip && <span className="badge badge--vip">VIP</span>}
      </div>
      <div className="event-card__body">
        <h3>{event.title}</h3>
        {event.subtitle && <p className="event-card__subtitle">{event.subtitle}</p>}
        <div className="event-card__meta">
          <span>{formatDate(event.date)}</span>
          <span>
            {event.location.city}
            {event.location.venue ? ` · ${event.location.venue}` : ''}
          </span>
        </div>
        <p className="event-card__fee">{formatPrice(event.registrationFee)}</p>
      </div>
    </Link>
  );
}
