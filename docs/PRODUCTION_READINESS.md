# Production readiness — the first pass

The work that sits between "demo runs locally" and "safe to point a load
balancer at." Not exhaustive — the prioritized first slice. Each item is
**why it matters** and **what we'd change**, no implementation detail.

The order is roughly the order I'd ship them.

1. [Postgres and migrations as a CI step](#1-postgres-and-migrations-as-a-ci-step)
2. [Secret management and key rotation](#2-secret-management-and-key-rotation)
3. [Auth hardening](#3-auth-hardening)
4. [Observability](#4-observability)
5. [Pagination and filtering on the events list](#5-pagination-and-filtering-on-the-events-list)
6. [Move event content to a headless CMS](#6-move-event-content-to-a-headless-cms)

---

## 1. Postgres and migrations as a CI step

**Why.** SQLite is a single-writer file. Two API instances behind a load
balancer pointing at the same file produce lock errors and can corrupt
data. The current "migrate on application startup" pattern also races
between instances during a rolling deploy — whichever container holds the
migration lock wins, the others fail to start.

**What.** Move to PostgreSQL (or SQL Server) and run schema migrations as
a discrete step in the deployment pipeline, before the new application
version goes live. The application no longer mutates the schema; it only
reads and writes data. This unblocks horizontal scaling and makes
migrations reviewable as part of the release.

---

## 2. Secret management and key rotation

**Why.** The JWT signing key currently lives in user-secrets, which is a
local-development convenience that does not exist on a deployed instance.
A static, never-rotated key also means a single compromise invalidates
every token issued and forces a redeploy to recover. The connection
string sits in the same place once we move off SQLite.

**What.** Move secrets to a managed store — Azure Key Vault, AWS Secrets
Manager, GCP Secret Manager, or HashiCorp Vault — and load them at
application startup. Establish a rotation cadence (quarterly is a
reasonable default) and design the bearer to accept multiple signing keys
during the rotation window so existing sessions don't break the moment
the active key flips.

---

## 3. Auth hardening

**Why.** The current implementation is the minimum that works:
unauthenticated callers can hit `/login` an unlimited number of times,
there's no account lockout, the access token is valid for 24 hours with
no way to revoke it, and there's no password reset flow at all.

**What.**

- **Rate limiting** on the auth endpoints, keyed by IP, to make
  credential stuffing infeasible.
- **Account lockout** after a small number of failed logins, with a clear
  unlock path.
- **Short-lived access tokens plus refresh tokens.** Refresh tokens are
  stored server-side (revocable on logout, rotated on each use, with
  reuse detection to catch token theft).
- **Password reset** via a single-use email-token flow.
- **HTTPS-only** with HSTS, and JWTs in `httpOnly` cookies rather than
  `localStorage` so a single XSS doesn't surrender the session.

---

## 4. Observability

**Why.** When a customer reports "the page is slow" or "I'm getting
errors," we currently have no way to find the request, correlate it
across log lines, see where time was spent, or know how often it's
happening. The `ILogger` calls go to console; that's it.

**What.** Adopt OpenTelemetry as the instrumentation layer (vendor-neutral
— ship to whichever backend the team picks). The minimum kit:

- **Structured logs** with trace IDs and per-request correlation,
  shipped to a log backend.
- **Metrics** for the things that matter: request rate and latency
  percentiles, error rates, registrations and (eventually) payments,
  JWT validation failures, cache hit rates.
- **Distributed tracing** on the hot paths so a slow request can be
  drilled into and the bottleneck found.
- **Health checks** — separate liveness (process is up) from readiness
  (dependencies are reachable), wired to the orchestrator and the load
  balancer respectively.
- **Error tracking** so unhandled exceptions trigger alerts with full
  context.

---

## 5. Pagination and filtering on the events list

**Why.** `GET /api/events` returns every event in one response. Fine at
three; broken at three thousand. Filtering by date range, city, VIP
status, and free-text are obvious next product asks, and retrofitting
them later is more painful than adding them now.

**What.** Add page/pageSize parameters with a sensible cap, return a paged
envelope (items + total + has-more) instead of a bare array. Add filter
parameters for the dimensions users actually want to slice by:
date range, location, VIP-only, free-text on title/subtitle, and
"currently registerable." Cap, validate, and version the contract.
Free-text search stays in Postgres until relevance becomes a real
requirement, at which point a dedicated search index earns its keep.

---

## 6. Move event content to a headless CMS

**Why.** The layout system has no authoring UI. Organizers can change
event metadata (title, dates, fee) but they cannot change the layout —
adding a SpeakerCard, reordering Sections, swapping in a new Heading —
without an engineer. Building that UI in-house is weeks of work and
mostly duplicates what off-the-shelf headless CMSes already do well.
Modular blocks in a CMS are exactly the polymorphic component tree we
built; their delivery API returns a pre-resolved tree with referenced
entities embedded.

**What.** Move event, speaker, session, and layout content to a headless
CMS (Contentstack, Sanity, or Payload depending on cost / hosting
preference). Keep auth, registration, payments, and the VIP gate in our
backend.

This swap removes a meaningful slice of code: the entities and
configurations for events / speakers / sessions / layouts /
LayoutComponent, the LayoutTreeBuilder, the create / update / read
event handlers, the seed data, and the corresponding repositories. What
stays: users, registrations, the auth flow, and a thin proxying read
endpoint that fetches from the CMS, applies the VIP gate against the
authenticated user's role, and caches.

The trade-off is real: vendor lock-in and a monthly bill, in exchange
for content-team independence, a built-in authoring UI, versioning,
drafts, scheduled publishing, asset management on a CDN, and roughly
half the backend code disappearing. For an event-organizing company
with a non-engineer content team, the trade-off is almost always worth
it.


