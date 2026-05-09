import { useEffect, useState } from 'react';
import { eventsApi } from '../api';
import type { EventSummary } from '../types';

interface State {
  events: EventSummary[];
  loading: boolean;
  error: Error | null;
}

export function useEvents(): State {
  const [state, setState] = useState<State>({ events: [], loading: true, error: null });

  useEffect(() => {
    let cancelled = false;
    setState({ events: [], loading: true, error: null });

    eventsApi
      .listEvents()
      .then((events) => {
        if (!cancelled) setState({ events, loading: false, error: null });
      })
      .catch((error: Error) => {
        if (!cancelled) setState({ events: [], loading: false, error });
      });

    return () => {
      cancelled = true;
    };
  }, []);

  return state;
}
