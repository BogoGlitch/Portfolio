# Portfolio — Project Guide

## What This Is
A personal portfolio site built to demonstrate enterprise-level full-stack engineering. The site itself is the showcase — architecture decisions are intentionally thorough so they can be explained and defended.

**Dual purpose:** Personal website + living proof of enterprise capability for potential employers/clients.

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

---

## Architecture Decisions

### Vertical Slice Architecture
Features live in `Features/` organized by operation, not by layer. Each feature folder contains its query/command record and its handler — nothing else.

**Why:** When a feature changes, you open one folder. No chasing logic across Controllers → Services → Repositories.

### No MediatR — Permanent Decision
Handlers are injected directly into controllers by concrete type. Explicit, readable, no indirection. MediatR was evaluated and rejected — it hides real dependencies behind `IMediator` and makes stack traces harder to follow in production.

### Explicit DI Registration
Everything in `Program.cs` is registered individually. No assembly scanning.

### Problem Details (RFC 7807)
All error responses: `{ type, title, status, detail, traceId }`.

### FluentValidation + ValidationFilter
`ValidationFilter<T>` is a generic action filter that runs before POST/PUT actions.

### CQRS Naming Convention
- **Queries** — read only, return DTO or null
- **Commands** — mutate data, return DTO or bool

### JWT Authentication
HttpOnly cookie, SameSite=Strict, Secure in prod. Token read via `OnMessageReceived`. Single admin user in appsettings. `[Authorize]` on all write endpoints.

### Serilog
Rolling file under `logs/` (14-day retention). EF Core and ASP.NET noise suppressed to Warning.

### Health Checks
- `GET /health` — full: process + database
- `GET /health/live` — liveness only

### Frontend Design System
Three themes toggled via ThemeToggle (cycles glitch → ember → cosmos):
- **Glitch** (default): blue/pink, oklch(65% 0.220 250) accent
- **Ember**: amber/crimson, oklch(72% 0.180 65) accent
- **Cosmos**: icy cyan/lavender, oklch(75% 0.110 195) accent

Theme switching: `data-theme` on `<html>`. FOUC prevented via inline `<script>` in layout.tsx plus `suppressHydrationWarning` on the `<html>` tag. Theme transitions isolated to `.theme-transitioning` class so hover interactions are unaffected.

**HeroTypewriter** — per-theme text effect: Glitch (character-by-character typewriter + block cursor + random symbol scramble), Ember (sentences roll up/in), Cosmos (sentences slide left/right). Cycling via `useCycler` hook for Ember/Cosmos; `useTypewriter` hook for Glitch only.

**ThemeToggle pill** — fixed width via CSS `::after` ghost holding the longest label ("glitch"), so the pill never shifts when "ember" is active. `useIsomorphicLayoutEffect` in `useTheme` syncs from localStorage before first paint — no flash on refresh.

**Scroll-reveal animations** — `AnimatedSection` uses `IntersectionObserver` (threshold 0.1, rootMargin -40px bottom). Animations fire as elements enter from below; reset when they exit via the bottom (scroll back up); elements that scroll off the top stay visible. Theme switch replays only currently-visible elements. Per-theme keyframes: glitch (slide-up + X snap), ember (perspective tilt fade-up), cosmos (clean fade-up).

**Hydration** — `useDocumentTheme` uses `useState('glitch')` + `useEffect` to avoid hydration mismatches. `suppressHydrationWarning` on `<html>` suppresses the FOUC script attribute diff.

**Header scroll behavior** — `position: fixed` (not sticky — sticky + backdrop-filter breaks transform). Hides on scroll down, reveals on any upward scroll via `useScrollDirection` hook (8px threshold). z-index: 400 so it always sits above the mobile drawer (300) and backdrop (200). `--header-height: 61px` in `:root` used by both `appMain` padding-top and drawer `top`.

**MobileNav** — Backdrop and drawer portaled to `document.body` via `createPortal` to escape the header's `backdrop-filter` stacking context (which caused the drawer background to render transparent). Scroll lock uses `position: fixed` on body + saves/restores `scrollY` — works on iOS Safari where `overflow: hidden` does not. Uses `useDocumentTheme` (not `useTheme`) so `data-theme` on the button stays in sync with the active theme — `useTheme` has isolated per-component state that doesn't update when another component calls `cycleTheme`.

**Hamburger animations** — Three completely different per-theme open+close keyframe animations. All use `animation-timing-function` inside keyframes for per-segment easing:
- **Glitch** — bars slide apart horizontally (top right, bottom left), middle fades, then cross diagonally back into X. Close reverses exactly.
- **Ember** — top 2 bars gather down to bottom bar (ease-in), then all spring up into X with overshoot (`cubic-bezier(0.34, 1.56, 0.64, 1)`). Close reverses.
- **Cosmos** — top+bottom slide left, middle slides right (all fade), invisible teleport to X-center at `scaleX(0)`, then X grows from a center dot. Close reverses (X shrinks to dot, bars shoot back out).

`data-open` / `data-closing` attributes on the button drive CSS selectors. JS state machine: `open` controls drawer/backdrop, `closing` controls hamburger reverse animation. `CLOSE_DURATION` record is theme-aware (`glitch: 280, ember: 420, cosmos: 520`) so the closing state clears precisely when each animation ends.

---

## Coding Conventions

- **Commits:** Small and frequent. One logical change per commit.
- **Explicit over magic:** Always choose the readable approach over the clever one.
- **PR workflow:** Every feature on its own branch → PR via `gh pr create` → merge → delete branch.
- **Error handling:** Handlers return `null` for not-found (→ 404). Throw `InvalidOperationException` for business rule violations (→ 500 via Problem Details).
- **Testing:** Smoke test all endpoints before opening a PR.

---

## Current State

### Branches
- `main` — stable, everything below is merged
- `feature/frontend-redesign` — active, frontend work in progress (not yet PRed)

### What's Done (Backend)
- [x] EF Core entities: Project, Technology, ProjectTechnology (join table)
- [x] LINQ projections for DTO mapping at query level
- [x] Problem Details (RFC 7807) error responses
- [x] FluentValidation with generic ValidationFilter<T>
- [x] Vertical slice handlers: Projects (2 queries, 3 commands)
- [x] Vertical slice handlers: Technologies (2 queries, 3 commands)
- [x] Controllers: HTTP-only, inject handlers directly by concrete type
- [x] Serilog: structured logging, console + rolling file
- [x] Health checks: /health and /health/live with JSON responses
- [x] JWT Bearer authentication via HttpOnly cookie (SameSite=Strict, Secure in prod)
- [x] POST /auth/login and POST /auth/logout endpoints
- [x] [Authorize] on all write endpoints (POST/PUT/DELETE)

### What's Done (Frontend)
- [x] Three-theme design system (glitch/ember/cosmos) with oklch() tokens
- [x] FOUC prevention, theme persistence via localStorage
- [x] Pages: Home, Projects (list + [slug]), Technologies (list + [slug])
- [x] Header: fixed position, hide-on-scroll-down / reveal-on-scroll-up (useScrollDirection), z-index 400
- [x] ThemeToggle: cycles themes, shows bolt/flame/moon-stars icon
- [x] CommandPalette: Cmd+K, server-fetched data, keyboard nav — also triggered by ⌘K button in header
- [x] MobileNav: portaled drawer, scroll lock (iOS-safe), hamburger animates open+close per theme (glitch/ember/cosmos each unique)
- [x] AnimatedSection: directional scroll-reveal per theme, replays on theme switch (visible only), resets on scroll-up
- [x] GlassCard: backdrop-filter, noise texture, hover lift
- [x] TechTag: accent-2 pill, isolated stacking context (no hover flicker)
- [x] TechIcon: react-icons/si mapping by slug, fallback to TbCode
- [x] GlowButton: primary/secondary/ghost variants
- [x] SkeletonLoader, HeroTypewriter: per-theme text effect (typewriter/roll-up/slide), prefers-reduced-motion aware
- [x] FilterPills (projects), CategoryTabs (technologies)
- [x] Headshot shape per theme: diagonal rounded corners (glitch), opposite diagonal (ember), circle (cosmos)
- [x] Stats bar: 2×2 grid on mobile (<640px), flex row on desktop
- [x] ThemeToggle pill: fixed width, no layout shift between themes
- [x] Hydration: suppressHydrationWarning on html, useIsomorphicLayoutEffect in useTheme

### Frontend — Still To Do
- [ ] Active nav link highlighting (current page underline stays visible)
- [ ] Headshot: actual photo (placeholder currently in use)

### Immediate Next
1. **PR + merge** — open PR for `feature/frontend-redesign`, merge to main
2. **Azure deployment** — App Service, Key Vault, CI/CD via GitHub Actions
3. **AI Job Fit feature** — user pastes job post, Claude/Azure OpenAI responds citing portfolio projects/technologies. Streaming response to frontend.
4. **Roles + multi-user auth** — add `role` claim to JWT, `[Authorize(Roles = "Admin")]`, Users table

---

## Key Files

| File | Purpose |
|---|---|
| `Portfolio.Api/Program.cs` | All DI registration and middleware pipeline |
| `Portfolio.Api/Features/` | All vertical slice handlers (queries + commands) |
| `Portfolio.Api/Filters/ValidationFilter.cs` | Generic pre-action validation filter |
| `Portfolio.Api/Validators/` | FluentValidation rules per DTO |
| `Portfolio.Api/Common/Projections/` | EF LINQ projections (entity → DTO at DB level) |
| `Portfolio.Api/Services/JwtTokenService.cs` | JWT signing implementation |
| `Portfolio.Api/Extensions/AuthenticationExtensions.cs` | JWT Bearer + cookie auth wiring |
| `Portfolio.Api/Extensions/SerilogConfiguration.cs` | Serilog setup |
| `Portfolio.Api/Extensions/HealthCheckExtensions.cs` | Health endpoint mapping |
| `portfolio-web/src/app/globals.css` | Design tokens (3 themes), entrance animation keyframes |
| `portfolio-web/src/app/layout.tsx` | Root layout: FOUC script, CommandPalette data fetch |
| `portfolio-web/src/app/page.tsx` | Home page: hero, stats bar, featured projects, explore grid |
| `portfolio-web/src/hooks/useTheme.ts` | Theme state + cycleTheme (writes data-theme + localStorage) |
| `portfolio-web/src/hooks/useDocumentTheme.ts` | Read-only theme via MutationObserver — use this (not useTheme) in any component that doesn't call cycleTheme |
| `portfolio-web/src/hooks/useScrollDirection.ts` | Returns `hidden: boolean` — true when scrolling down, false on any upward scroll |
| `portfolio-web/src/app/components/` | All shared UI components |
| `portfolio-web/src/lib/api.ts` | Frontend API client |
| `README.md` | Project overview, setup instructions, endpoint reference |
| `ARCHITECTURE.md` | Design decisions and reasoning |
| `DEVLOG.md` | Local-only chronological dev journal — not source controlled |

---

## Documentation Maintenance

When completing a feature or making an architectural decision, update:

- **`CLAUDE.md`** — Current State, Immediate Next, Key Files
- **`README.md`** — new endpoints, setup changes, structure changes
- **`ARCHITECTURE.md`** — new patterns or significant decisions
- **`DEVLOG.md`** — append entry summarising what was built and why
