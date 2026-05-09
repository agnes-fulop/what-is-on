import type { EventDetail, EventSummary } from '../types';
import { apiFetch } from './client';

export function listEvents(): Promise<EventSummary[]> {
  return apiFetch<EventSummary[]>('/api/events');
}

export function getEvent(id: string): Promise<EventDetail> {
  return apiFetch<EventDetail>(`/api/events/${id}`);
}

export interface SessionInput {
  title: string;
  description: string;
  from: string;
  to: string;
  level: 'Beginner' | 'Intermediate' | 'Advanced';
  track: string;
  room: string;
  speakerId: string | null;
}

export interface EventInput {
  title: string;
  subtitle: string;
  description: string;
  isVip: boolean;
  date: string;
  hero: { image: string; ctaText: string };
  location: { city: string; venue: string; address: string };
  registration: {
    openDate: string;
    closeDate: string;
    fee: number;
    earlyBirdDiscount: number;
  };
  layoutId: string | null;
}

export interface CreateEventInput extends EventInput {
  sessions: SessionInput[] | null;
}

export function createEvent(input: CreateEventInput): Promise<{ id: string }> {
  return apiFetch<{ id: string }>('/api/events', {
    method: 'POST',
    body: input,
  });
}

export function updateEvent(id: string, input: EventInput): Promise<void> {
  return apiFetch<void>(`/api/events/${id}`, {
    method: 'PUT',
    body: input,
  });
}
