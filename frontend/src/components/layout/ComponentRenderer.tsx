import type {
  HeadingLevel,
  LayoutComponent,
  Session,
  Speaker,
} from '../../types';

/**
 * Recursive renderer that switches on component.type. Container types
 * (Section, SpeakerList, SessionSchedule) recurse via this same component
 * so the tree is traversed top-down without each renderer needing to know
 * about the others.
 */
export function ComponentRenderer({ component }: { component: LayoutComponent }) {
  switch (component.type) {
    case 'Section':
      return (
        <section className={`section section--${component.data.direction}`}>
          {component.children.map(renderChild)}
        </section>
      );

    case 'Heading': {
      const Tag = component.data.level as HeadingLevel;
      return <Tag>{component.data.text}</Tag>;
    }

    case 'Paragraph':
      return <p style={{ whiteSpace: 'pre-line' }}>{component.data.text}</p>;

    case 'SpeakerList':
      return (
        <section className="speaker-list">
          <h3>{component.data.title}</h3>
          <div className="speaker-list__cards">{component.children.map(renderChild)}</div>
        </section>
      );

    case 'SpeakerCard':
      return <SpeakerCardView speaker={component.data.speaker} />;

    case 'SessionSchedule':
      return (
        <section className="session-schedule">
          <h3>{component.data.title}</h3>
          <div className="session-schedule__cards">{component.children.map(renderChild)}</div>
        </section>
      );

    case 'SessionCard':
      return <SessionCardView session={component.data.session} />;
  }
}

function renderChild(child: LayoutComponent) {
  return <ComponentRenderer key={child.id} component={child} />;
}

function SpeakerCardView({ speaker }: { speaker: Speaker | null }) {
  if (!speaker) {
    return <div className="speaker-card muted">Speaker unavailable</div>;
  }
  return (
    <div className="speaker-card">
      {speaker.image ? (
        <img className="speaker-card__avatar" src={speaker.image} alt="" />
      ) : (
        <div className="speaker-card__avatar" aria-hidden="true" />
      )}
      <p className="speaker-card__name">{speaker.name}</p>
      <p className="speaker-card__title">{speaker.title}</p>
    </div>
  );
}

function SessionCardView({ session }: { session: Session | null }) {
  if (!session) {
    return <div className="session-card muted">Session unavailable</div>;
  }
  return (
    <div className="session-card">
      <div className="session-card__time">
        {formatTime(session.from)}–{formatTime(session.to)}
      </div>
      <div>
        <p className="session-card__title">{session.title}</p>
        <p className="session-card__meta">
          {session.room}
          {session.speaker ? ` · ${session.speaker.name}` : ''}
          {session.level ? ` · ${session.level}` : ''}
        </p>
      </div>
    </div>
  );
}

function formatTime(iso: string): string {
  const date = new Date(iso);
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}
