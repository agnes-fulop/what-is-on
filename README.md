# What's On

An event platform with a flexible, template-driven layout system. Event pages
are built from a tree of nested, reusable components (Section, Heading,
Paragraph, SpeakerList, SpeakerCard, SessionSchedule, SessionCard) stored
server-side and rendered client-side. Supports VIP-gated events, user
registration, and organizer-only event editing.

## Contents

- [Stack](#stack)
- [Quick start with Docker](#quick-start-with-docker)
- [Prerequisites (manual setup)](#prerequisites-manual-setup)
- [Setup](#setup)
  - [Backend](#1-backend)
  - [Frontend](#2-frontend)
- [Run](#run)
- [Demo credentials](#demo-credentials)
- [Endpoints](#endpoints)
- [Project structure](#project-structure)
- [Notes](#notes)
- [Key decisions](#key-decisions)
- [Going to production](#going-to-production)

## Stack

- **Backend** — .NET 10 ASP.NET Core Web API (mature, strongly typed,
  performant), EF Core 10 + SQLite (zero-setup local dev), JWT bearer auth
  (stateless, horizontally scalable), BCrypt password hashing (industry
  standard, adjustable work factor)
- **Frontend** — React 19 + TypeScript + Vite (fast HMR, end-to-end type
  safety with the API), Zustand for auth state (tiny, no Redux boilerplate),
  native `fetch` (no Axios — fewer deps, sufficient API surface)
- **Build** — single .NET project (`backend/WhatIsOn.Api/`, less ceremony
  than a four-project clean-architecture split at this scale) and a Vite app
  (`frontend/`)

## Quick start with Docker

If you have Docker Desktop (or any Docker Engine with Compose v2):

```sh
docker compose up --build
```

That's it. The frontend is at <http://localhost:5173> and the backend at
<http://localhost:5051>. The frontend container's nginx proxies `/api` calls
to the backend, so the bundle uses relative URLs and CORS is moot in this
setup. SQLite data persists in a named volume (`whatison-data`) across
restarts.

The compose file ships with a demo JWT signing key for local convenience.
Override it before bringing the stack up if you'd rather not use the bundled
default:

```sh
# macOS / Linux
JWT_KEY="$(openssl rand -base64 48)" docker compose up --build
```

```powershell
# Windows PowerShell
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$bytes = New-Object byte[] 48; $rng.GetBytes($bytes)
$env:JWT_KEY = [Convert]::ToBase64String($bytes)
docker compose up --build
```

To reset the demo database, run `docker compose down -v` to drop the volume,
then bring the stack back up.

If you'd rather run the services on the host (faster startup, hot reload),
follow the manual setup below instead.

## Prerequisites (manual setup)

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (10.0.203+)
- [Node.js 20+](https://nodejs.org) (tested on 24)
- The `dotnet-ef` global tool (only needed if you want to run migrations
  manually — the API applies pending migrations on startup)

## Setup

### 1. Backend

```sh
cd backend/WhatIsOn.Api
dotnet restore
```

The API needs a JWT signing key. It's loaded from user-secrets in development
and the API will fail-fast at startup if it's missing. Generate a random key
once per machine:

**Windows (PowerShell):**
```powershell
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$bytes = New-Object byte[] 48; $rng.GetBytes($bytes)
dotnet user-secrets set "Jwt:Key" ([Convert]::ToBase64String($bytes))
```

**macOS / Linux:**
```sh
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 48)"
```

The key lives in `%APPDATA%\Microsoft\UserSecrets\<id>\secrets.json` (Windows)
or `~/.microsoft/usersecrets/<id>/secrets.json` (macOS/Linux) — it is **not**
committed to the repo.

### 2. Frontend

```sh
cd frontend
npm install
```

The frontend reads the API URL from `VITE_API_URL`. The committed `.env` points
at `http://localhost:5051` by default; override locally via `.env.local` if
needed.

## Run

Open two terminals.

**Terminal 1 — backend:**
```sh
cd backend/WhatIsOn.Api
dotnet run --launch-profile http
```

The API listens on `http://localhost:5051`. On first run it creates
`whatison.db`, applies migrations, and seeds a demo catalog (4 events, 6
speakers, 4 users, 3 layouts). Subsequent runs are no-ops.

**Terminal 2 — frontend:**
```sh
cd frontend
npm run dev
```

The Vite dev server listens on `http://localhost:5173`.

## Demo credentials

All demo accounts share the password `demo-password-123`:

| Email | Role |
|---|---|
| `regular@example.com` | Regular — can view non-VIP events and register |
| `vip@example.com` | VIP — additionally can view and register for VIP events |
| `organizer@example.com` | Organizer — can create/edit events, owns the seeded events |
| `organizer2@example.com` | Organizer — owns the past Tech Summit; useful for verifying ownership checks |

## Endpoints

| Method | Path | Auth |
|---|---|---|
| POST | `/api/auth/register` | — |
| POST | `/api/auth/login` | — |
| GET | `/api/events` | — |
| GET | `/api/events/{id}` | Conditional — VIP events require an authenticated VIP/Organizer |
| POST | `/api/events/{id}/registrations` | Any authenticated user |
| POST | `/api/events` | Organizer |
| PUT | `/api/events/{id}` | Organizer (must own the event) |

OpenAPI spec is served at `/openapi/v1.json` in development.

## Project structure

```
what-is-on/
  WhatIsOn.slnx
  docker-compose.yml
  backend/
    WhatIsOn.Api/
      Dockerfile
      Application/         use cases (Auth, Events, Registrations) + DTOs
      Authorization/       JWT bearer wiring + policy constants
      Controllers/         AuthController, EventsController, RegistrationsController
      Domain/              entities, enums, value objects, repository interfaces, exceptions
      Infrastructure/
        Authentication/    BCrypt + JWT implementations
        Persistence/       DbContext, EF configurations, migrations, seed
        Repositories/      EF Core repository implementations
      Middleware/          ExceptionHandlingMiddleware (RFC 7807)
      Services/            CurrentUser (reads ClaimsPrincipal)
  frontend/
    Dockerfile
    nginx.conf             SPA fallback + /api reverse-proxy to backend
    src/
      api/                 typed fetch wrapper + per-resource modules
      components/          Navbar, EventCard, ProtectedRoute, SubmitButton, layout/ComponentRenderer
      hooks/               useEvents, useEventDetail
      pages/               HomePage, EventDetailPage, LoginPage, RegisterPage
      store/               authStore (Zustand)
      types/               TypeScript mirrors of API DTOs
      utils/               formatting helpers
```

## Notes

- The SQLite file (`whatison.db`) and `node_modules/` are gitignored.
- Resetting the demo database: stop the API, delete `whatison.db` (and the
  `*-shm` / `*-wal` siblings if present), and restart — the seed runs again.
- The seed is idempotent at the API level: it only inserts when the events
  table is empty.

## Key decisions

- **Single .NET project, layered by namespace.** Chose folder / namespace
  separation over a four-project Clean Architecture solution
  (`Domain` / `Application` / `Infrastructure` / `Api`). Less ceremony for a
  codebase this size; the cost is losing compile-time enforcement of layer
  boundaries — discipline is enforced by review, not the build.

- **Adjacency-list rows plus a polymorphic JSON `Data` column for the
  layout tree.** Each layout component is one row with `ParentComponentId`
  + `SortOrder` and a small JSON blob shaped per component type. Simple
  schema, trivial to add new component types, queryable at the row level.
  The cost: reconstructing the tree at read time (handled in one place by
  `LayoutTreeBuilder`) and the JSON contents are opaque to the database.

- **SQLite for development, JWT in `localStorage` on the client.** The
  textbook-simple defaults so the demo runs zero-setup. Both have well-known
  production constraints (multi-instance contention for SQLite, XSS
  exposure for `localStorage` tokens) — addressed in
  [docs/PRODUCTION_READINESS.md](docs/PRODUCTION_READINESS.md), not in the
  demo itself.

- **xUnit + Shouldly + NSubstitute for tests.** Chose this over the more
  conventional FluentAssertions + Moq. Avoids FluentAssertions v8's
  commercial license and the Moq telemetry incident; the trade-off is
  less prevalence in tutorials and Stack Overflow answers.

## Going to production

The codebase as it stands is positioned as a working demo. The first slice
of work to take it to production — Postgres + migrations as a CI step,
secret management with key rotation, auth hardening (rate limiting, account
lockout, refresh tokens), an observability stack, paginated/filterable
event listing, and moving event content to a headless CMS instead of
building a layout authoring UI — is documented in
[docs/PRODUCTION_READINESS.md](docs/PRODUCTION_READINESS.md).
