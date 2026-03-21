# Architecture & Design Decisions

This document records the architectural decisions made in this project and the reasoning behind each one. It exists so decisions can be explained, defended, and revisited with full context.

---

## Vertical Slice Architecture

Features live in `Features/` organized by operation, not by layer.

```
Features/
  Projects/
    Queries/GetProjects/
    Queries/GetProjectBySlug/
    Commands/CreateProject/
    Commands/UpdateProject/
    Commands/DeleteProject/
  Technologies/
    (same structure)
  Auth/
    Commands/Login/
```

Each folder contains exactly one record (the command/query) and one handler. Nothing else.

**Why:** When a feature changes, you open one folder. No tracing logic across Controllers → Services → Repositories → back. The blast radius of any change is contained to its slice.

---

## No MediatR — Intentional

Handlers are injected directly into controllers by concrete type. There is no mediator bus.

```csharp
public ProjectsController(
    GetProjectsQueryHandler getProjects,
    CreateProjectCommandHandler createProject, ...)
```

**Why not MediatR:** MediatR hides dependencies behind `IMediator`. You inject one thing and lose visibility into what the controller actually needs. Stack traces route through the bus rather than directly to the handler. A colleague at Microsoft flagged this as a real traceability problem in production systems.

**How we handle cross-cutting concerns without it:**
- Validation: `ValidationFilter<T>` as a registered action filter — runs before any handler, explicit per-action via `[ServiceFilter]`
- Logging: handlers receive `ILogger<T>` directly — visible, traceable
- Transactions: explicit `using var tx = ...` in command handlers where needed

The rule: add abstraction when you feel the friction of not having it, not before.

---

## Explicit DI Registration

Every dependency is registered individually in `Program.cs`. No assembly scanning, no convention-based auto-wiring.

**Why:** `Program.cs` is the table of contents for the application. Anyone reading it knows exactly what the app can do and what it depends on. Assembly scanning is magic — it hides registrations and makes debugging DI failures harder.

---

## Extension Methods Keep Program.cs Clean

All non-trivial service registration lives in extension methods under `Extensions/`:

| Extension | Responsibility |
|---|---|
| `SerilogConfiguration.Configure` | Serilog setup — sinks, filters, output templates |
| `HealthCheckExtensions.MapHealthCheckEndpoints` | Health route mapping |
| `AuthenticationExtensions.AddJwtCookieAuthentication` | JWT Bearer + cookie config, auth service registrations |

`Program.cs` calls these as one-liners. It stays readable as a high-level map of the application.

---

## Problem Details (RFC 7807)

All error responses follow the RFC 7807 shape:

```json
{
  "type": "https://...",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "00-abc..."
}
```

Configured via `AddProblemDetails()` + `UseExceptionHandler()` + `UseStatusCodePages()`.

**Why:** Consistent, machine-readable error responses across every status code. Clients never have to guess the error shape.

---

## FluentValidation + ValidationFilter

`ValidationFilter<T>` is a generic action filter that validates the request DTO before the handler runs. Applied per-action via `[ServiceFilter(typeof(ValidationFilter<CreateProjectDto>))]`.

**Why a filter over inline validation:** Validation is cross-cutting. A filter centralises the 400 response logic so handlers never have to deal with shape validation — they only receive valid input. Handlers focus on business rules.

**Division of responsibility:**
- Shape validation (required fields, length, format) → validators + filter
- Business rule validation (slug uniqueness, FK checks) → command handlers

---

## CQRS Naming Convention

- **Queries** — read-only, return DTO or null, never mutate state
- **Commands** — mutate data, return DTO or bool

This isn't full CQRS with separate read/write models — it's the naming convention and separation of intent. Handlers are not abstracted behind an interface; they are concrete classes injected by type.

---

## Authentication — JWT Bearer via HttpOnly Cookie

**Authentication** (who are you) and **Authorization** (what can you do) are treated as distinct concerns.

### Flow
1. `POST /api/auth/login` — credentials validated against `AdminCredentials` in configuration
2. `LoginCommandHandler` calls `ITokenService.GenerateToken(username)`
3. `JwtTokenService` signs a JWT (HS256) with claims: `sub`, `jti`, `iat`
4. `AuthController` writes the token into an HttpOnly cookie on the response

### Cookie settings
| Setting | Value | Reason |
|---|---|---|
| `HttpOnly` | `true` | JavaScript cannot read the token — XSS cannot steal it |
| `Secure` | `true` in production, `false` in development | HTTPS-only in prod; relaxed locally to support HTTP dev profile |
| `SameSite` | `Strict` | Cookie only sent on same-site requests — prevents CSRF |

### Why HttpOnly cookie over localStorage
`localStorage` is readable by any JavaScript on the page. If a dependency is compromised or a DOM injection vulnerability exists, the token can be exfiltrated. An HttpOnly cookie cannot be read by JS at all — only the browser sends it, automatically, on matching requests.

### Token validation
The JwtBearer middleware is configured to read the token from the cookie rather than the `Authorization` header via `OnMessageReceived`. All validation (issuer, audience, lifetime, signature) is handled by the middleware — controllers and handlers are unaware of JWT mechanics.

### Authorization
`[Authorize]` on write endpoints (POST/PUT/DELETE). Read endpoints (GET) are public — the portfolio displays without authentication.

### Future path
- **Roles:** Add a `role` claim to the token. Replace `[Authorize]` with `[Authorize(Roles = "Admin")]`. No pipeline changes.
- **Multiple users:** Replace config-based credential check in `LoginCommandHandler` with a `Users` table lookup. No controller or middleware changes.
- **Azure AD / Entra ID:** Point the JWT validation at Azure AD as the issuer. `ITokenService` and `LoginCommandHandler` are replaced; `[Authorize]` attributes are unchanged.

---

## Serilog

Configured in `Extensions/SerilogConfiguration.cs`, not in `Program.cs`.

- **Console sink** — development
- **Rolling file sink** — `logs/` directory, 14-day retention
- EF Core and ASP.NET Core framework logs suppressed to `Warning` — only application-level logs appear at `Information`

**Why suppress framework logs:** EF Core is extremely chatty at Information level. Suppressing it means the log output is signal, not noise.

---

## Health Checks

- `GET /health` — full check: process alive + EF Core DbContext query against SQL Server
- `GET /health/live` — liveness only: process up, no dependency checks

**Why two endpoints:** Liveness (`/health/live`) tells an orchestrator the process is running. Readiness (`/health`) tells it the process can serve traffic. These are different questions and should have different answers during startup or DB outages.

---

## Frontend Design System

### Three-Theme System

All colors are oklch() tokens set as CSS custom properties on `[data-theme]` selectors in `globals.css`. Three themes: `glitch` (blue/pink), `ember` (amber/crimson), `cosmos` (cyan/lavender). `useTheme` writes `data-theme` to the DOM and localStorage. Components read the theme via `data-theme` CSS selectors or the `useDocumentTheme` hook.

### FOUC Prevention

An inline `<script>` in `layout.tsx` reads localStorage and sets `data-theme` before React hydrates — the page renders in the correct theme immediately. `suppressHydrationWarning` on `<html>` suppresses the React mismatch warning this causes (intentional, not a bug).

### Hydration Strategy

`useTheme` uses `useIsomorphicLayoutEffect` (useLayoutEffect on client, useEffect on server) to sync from localStorage before first paint — eliminates the theme pill flash. `useDocumentTheme` uses `useState('glitch')` + `useEffect` to avoid React hydration mismatches from reading the DOM during render.

### Scroll-Reveal Animations (AnimatedSection)

`AnimatedSection` wraps `IntersectionObserver` with directional awareness:
- Elements animate in when entering from below (threshold 0.1, rootMargin -40px bottom)
- Elements reset when they exit via the bottom (user scrolled back up)
- Elements that scroll off the top stay visible — only one direction resets
- Theme switch replays animations only on currently-visible elements

Per-theme keyframes in `globals.css`: glitch (slide-up + X snap), ember (perspective tilt), cosmos (clean fade-up). Animation-name change on the element triggers browser replay — no JS needed to restart the animation.

### Per-Theme HeroTypewriter

Three completely different text effects driven by `useDocumentTheme`:
- **Glitch** — character-by-character typewriter, block cursor, periodic symbol scramble
- **Ember** — sentences roll up and out, next rolls in from below (`useCycler` hook)
- **Cosmos** — sentences slide out right, next slides in from left (`useCycler` hook)
