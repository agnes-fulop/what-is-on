import type { LayoutDto } from './layout';

export type SessionLevel = 'Beginner' | 'Intermediate' | 'Advanced';

export interface Hero {
  image: string;
  ctaText: string;
}

export interface Location {
  city: string;
  venue: string;
  address: string;
}

export interface Organizer {
  id: string;
  name: string;
}

export interface Speaker {
  id: string;
  name: string;
  title: string;
  bio: string;
  image: string;
}

export interface Session {
  id: string;
  title: string;
  description: string;
  from: string;
  to: string;
  level: SessionLevel;
  track: string;
  room: string;
  speaker: Speaker | null;
}

export interface RegistrationInfo {
  openDate: string;
  closeDate: string;
  fee: number;
  earlyBirdDiscount: number;
}

export interface EventSummary {
  id: string;
  title: string;
  subtitle: string;
  date: string;
  isVip: boolean;
  hero: Hero;
  location: Location;
  organizer: Organizer;
  registrationFee: number;
  earlyBirdDiscount: number;
}

export interface EventDetail {
  id: string;
  title: string;
  subtitle: string;
  description: string;
  isVip: boolean;
  date: string;
  hero: Hero;
  location: Location;
  organizer: Organizer;
  sessions: Session[];
  registration: RegistrationInfo;
  layout: LayoutDto | null;
}

export interface RegistrationResult {
  id: string;
  eventId: string;
  userId: string;
  registeredAt: string;
}
