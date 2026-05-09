import { useCallback, useEffect, useState } from 'react';
import { eventsApi } from '../api';
import { ApiError } from '../api/client';
import type { EventDetail } from '../types';

interface State {
  event: EventDetail | null;
  loading: boolean;
  error: ApiError | Error | null;
}

interface UseEventDetailResult extends State {
  reload: () => void;
}

export function useEventDetail(id: string): UseEventDetailResult {
  const [state, setState] = useState<State>({ event: null, loading: true, error: null });
  const [reloadCounter, setReloadCounter] = useState(0);

  const reload = useCallback(() => setReloadCounter((n) => n + 1), []);

  useEffect(() => {
    let cancelled = false;
    setState({ event: null, loading: true, error: null });

    eventsApi
      .getEvent(id)
      .then((event) => {
        if (!cancelled) setState({ event, loading: false, error: null });
      })
      .catch((error: Error) => {
        if (!cancelled) setState({ event: null, loading: false, error });
      });

    return () => {
      cancelled = true;
    };
  }, [id, reloadCounter]);

  return { ...state, reload };
}
