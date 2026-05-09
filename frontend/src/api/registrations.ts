import type { RegistrationResult } from '../types';
import { apiFetch } from './client';

export function registerForEvent(eventId: string): Promise<RegistrationResult> {
  return apiFetch<RegistrationResult>(`/api/events/${eventId}/registrations`, {
    method: 'POST',
  });
}
