# Portfolio — Project Guide

## What This Is
Personal portfolio site + living proof of enterprise-level full-stack engineering. Every decision can be explained and defended.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Web API, .NET 10 |
| ORM | Entity Framework Core 10, SQL Server |
| Validation | FluentValidation 11 |
| Logging | Serilog 9 (console + file sinks) |
| Frontend | Next.js 16 (App Router), React 19, TypeScript |
| Styling | CSS Modules, oklch() color space |
| Icons | react-icons (tb, si sets) |
| Hosting | Azure — everything (API, frontend, DB, auth, secrets) |
| CI/CD | GitHub Actions → Azure (build, test, deploy pipelines) |

---

## Hosting Strategy — Azure Everything

**All infrastructure lives in Azure. No third-party hosting.** This mirrors enterprise reality and is the deliberate learning goal. Target: enterprise-grade Azure fluency for under $50/month.

Why Azure Static Web Apps over Vercel: SWA integrates natively with GitHub Actions, Azure AD, and the Azure ecosystem. The friction is higher — that's the point.

**CI/CD philosophy:** GitHub Actions for every deployment. No manual `az` deploys after initial setup. Build → test → deploy on every merge to `main`.

---

## Architecture — Critical Rules

**No MediatR — permanent.** Handlers injected directly into controllers by concrete type. MediatR hides real dependencies and makes stack traces harder in prod.

**Explicit DI only.** Everything registered individually in `Program.cs`. No assembly scanning.

**Vertical slice.** Features live in `Features/` by operation. Each folder: one record + one handler, nothing else.

**Error handling.** Handlers return `null` for not-found (→ 404). Throw `InvalidOperationException` for business rule violations (→ 500 via Problem Details / RFC 7807).

**Validation split.** Shape validation (required, length, format) → FluentValidation + `ValidationFilter<T>`. Business rules → command handlers.

---

## Frontend — Critical Gotchas

**`useDocumentTheme` not `useTheme` in most components.** `useTheme` has isolated per-component state. `useDocumentTheme` watches `html[data-theme]` via MutationObserver. Only `ThemeToggle` uses `useTheme`.

**Header is `position: fixed`, not `position: sticky`.** Sticky + `backdrop-filter` breaks `transform`. `--header-height: 61px` in `:root` is the single source of truth.

**MobileNav must portal to `document.body`.** The header's `backdrop-filter` creates a stacking context that traps fixed children.

**iOS Safari scroll lock.** `overflow: hidden` on body doesn't work on iOS. Use `position: fixed; top: -${y}px` pattern.

**Theme system.** Four themes: `cosmos` (default), `glitch`, `ember`, `prism` (light). All oklch() tokens on `[data-theme]` selectors in `globals.css`. FOUC prevented via inline `<script>` in `layout.tsx`.

---

## Coding Conventions

- **Commits:** Small and frequent. One logical change per commit.
- **Explicit over magic:** Readable over clever, always.
- **PR workflow:** Feature branch → `gh pr create` → merge → delete branch.
- **Testing:** Smoke test all endpoints before opening a PR.

---

## Current State

### Backend
- **132 tests passing** — all handlers, validators, and config tests
- **Known issue:** 2 tests commented out — in-memory DB name conflicts (prefixed Create_/Update_ on active tests; commented-out tests still need similar fix).
- **Resilience stack:** EF Core retry (5 retries, 30s backoff) + `DatabaseKeepAliveService` (SQL ping every 4min) + `QueryWarmupService` (pre-compiles EF queries at startup)
- **Observability:** Application Insights (requests, SQL dependencies, exceptions), Serilog (console + file), App Insights daily cap 0.1 GB/day, availability test every 5min

### Frontend
- **Admin UI:** Full CRUD for Skills and Projects at `/admin/*`
- **Filter system:** ProjectFilterModal (3-level cascading), Skills discipline pill bar (server component)
- **SSR resilience:** fetchJson with 15s timeout + 1 retry, allSettled fallbacks, root error boundary

### Immediate Next
1. **Reseed Skills** — clean up DB, remove resume-padding entries, establish a solid foundation before deeper work
2. **Skills landing page** — review layout/filters once reseeded data is in place

### Backlog
- [ ] Active nav link / current-page state (useful even with future menu system)
- [ ] Breadcrumb component — own container, not tied to headline component
- [ ] Skill detail pages — back burner, revisit after Skills cleanup
- [ ] Theme cycle order — default to Prism (light), reorder cycle based on `prefers-color-scheme`
- [ ] Caching / avoid unnecessary re-renders — HTTP cache headers on API, verify Next.js router cache, explore Azure Cache for Redis
- [ ] **AI Job Fit feature** — user pastes job post, Azure OpenAI responds citing portfolio projects. Streaming response. Needs a gated role (e.g. "JobSeeker") to control costs.
- [ ] **Entra ID exploration** — understand Entra ID (workforce SSO) vs External ID (customer-facing). External ID for Job Fit sign-up, Entra ID for admin.
- [ ] **Roles + multi-user auth** — `role` claim in JWT, `[Authorize(Roles = "Admin")]`, Users table

---

## Key Files

| File | Purpose |
|---|---|
| `Portfolio.Api/Program.cs` | All DI registration and middleware pipeline |
| `Portfolio.Api/Features/` | All vertical slice handlers |
| `Portfolio.Api/Filters/ValidationFilter.cs` | Generic pre-action validation filter |
| `Portfolio.Api/Common/Projections/` | EF LINQ projections (entity → DTO at query level) |
| `Portfolio.Api/Extensions/AuthenticationExtensions.cs` | JWT Bearer + cookie auth wiring |
| `Portfolio.Api/Services/QueryWarmupService.cs` | Pre-compiles EF queries at startup |
| `Portfolio.Api/Services/DatabaseKeepAliveService.cs` | Pings SQL every 4min to prevent idle timeout |
| `portfolio-web/src/app/globals.css` | All design tokens (4 themes) + animation keyframes |
| `portfolio-web/src/app/layout.tsx` | Root layout: FOUC script, CommandPalette data fetch |
| `portfolio-web/src/hooks/useDocumentTheme.ts` | Read-only theme via MutationObserver — use everywhere except ThemeToggle |
| `portfolio-web/src/app/components/MobileNav.tsx` | Portaled drawer, scroll lock, hamburger state machine |
| `portfolio-web/src/lib/api.ts` | Typed frontend API client |
| `portfolio-web/src/context/AuthContext.tsx` | Shared auth state — consumed by admin layout |
| `portfolio-web/src/app/components/ProjectFilterModal.tsx` | Cascading filter modal (Discipline → Category → Skill) |
| `ARCHITECTURE.md` | Design decisions with reasoning |
| `INTERVIEW.md` | Interview prep — decisions with "why, what over, what at scale" (local only) |
| `DEVLOG.md` | Chronological dev journal (local only) |

---

## Documentation Maintenance

**After each commit**, evaluate whether the change is worth documenting. If it introduced a new pattern, architectural decision, operational change, or lesson learned, update the relevant files before moving on:

- **`DEVLOG.md`** — append chronological entry (what happened, what broke, what we learned)
- **`ARCHITECTURE.md`** — new patterns or significant technical decisions
- **`INTERVIEW.md`** — new sections with: what, why, chose over, at scale, "how I'd say it"
- **`CLAUDE.md`** — update Current State, Immediate Next, Key Files as needed
- **`README.md`** — new endpoints or setup changes

Not every commit needs all five — use judgment. A CSS tweak needs nothing. A new service or architecture decision needs all of them. **Don't wait for the user to ask.**
