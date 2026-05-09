import { useAuthStore } from '../store/authStore';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5051';

/**
 * Thrown for any non-2xx response. Carries the parsed RFC 7807 problem+json
 * fields so callers can decide how to display the error.
 */
export class ApiError extends Error {
  readonly status: number;
  readonly title: string;
  readonly detail: string;
  readonly raw: unknown;

  constructor(status: number, title: string, detail: string, raw: unknown) {
    super(detail || title || `HTTP ${status}`);
    this.name = 'ApiError';
    this.status = status;
    this.title = title;
    this.detail = detail;
    this.raw = raw;
  }
}

interface RequestOptions extends Omit<RequestInit, 'body'> {
  body?: unknown;
}

export async function apiFetch<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { body, headers: rawHeaders, ...rest } = options;
  const headers = new Headers(rawHeaders);

  const token = useAuthStore.getState().token;
  if (token) {
    headers.set('Authorization', `Bearer ${token}`);
  }

  let serializedBody: string | undefined;
  if (body !== undefined) {
    serializedBody = typeof body === 'string' ? body : JSON.stringify(body);
    if (!headers.has('Content-Type')) {
      headers.set('Content-Type', 'application/json');
    }
  }

  const response = await fetch(`${BASE_URL}${path}`, {
    ...rest,
    headers,
    body: serializedBody,
  });

  if (!response.ok) {
    throw await parseError(response);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

async function parseError(response: Response): Promise<ApiError> {
  let problem: { title?: string; detail?: string } | null = null;
  try {
    const text = await response.text();
    if (text) {
      problem = JSON.parse(text);
    }
  } catch {
    /* response body was not JSON — fall through */
  }

  return new ApiError(
    response.status,
    problem?.title ?? response.statusText ?? 'Request failed',
    problem?.detail ?? '',
    problem,
  );
}
