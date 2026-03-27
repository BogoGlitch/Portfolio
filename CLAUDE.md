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

**All infrastructure lives in Azure. No third-party hosting (no Vercel, no Netlify, no Railway).** This mirrors what large enterprises actually run and is the deliberate learning goal.

| Service | Azure Resource | Est. Cost |
|---|---|---|
| API | App Service (B1 Linux) | ~$13/mo |
| Frontend | Static Web Apps (Standard) | ~$9/mo |
| Database | Azure SQL (Basic, 5 DTU) | ~$5/mo |
| Secrets | Key Vault | ~$0/mo (free ops volume) |
| Auth (future) | Azure AD B2C or Entra ID | free tier sufficient |
| **Total** | | **~$27/mo** |

**Target: enterprise-grade Azure fluency for under $50/month.**

Why Azure Static Web Apps over Vercel: SWA is what enterprises with Azure-standardized infra use. It integrates natively with GitHub Actions, Azure AD, and the rest of the Azure ecosystem. The friction is higher than Vercel but that's the point — learning the real thing.

**CI/CD philosophy:** Azure Pipelines YAML (or GitHub Actions targeting Azure) for every deployment. No manual `az` deploys after initial setup. Build → test → deploy on every merge to `main`.

---

## Architecture — Critical Rules

**No MediatR — permanent.** Handlers injected directly into controllers by concrete type. MediatR hides real dependencies and makes stack traces harder in prod.

**Explicit DI only.** Everything registered individually in `Program.cs`. No assembly scanning.

**Vertical slice.** Features live in `Features/` by operation. Each folder: one record + one handler, nothing else.

**Error handling.** Handlers return `null` for not-found (→ 404). Throw `InvalidOperationException` for business rule violations (→ 500 via Problem Details / RFC 7807).

**Validation split.** Shape validation (required, length, format) → FluentValidation + `ValidationFilter<T>`. Business rules → command handlers.

---

## Frontend — Critical Gotchas

**`useDocumentTheme` not `useTheme` in most components.** `useTheme` has isolated per-component state — it won't receive updates when another component calls `cycleTheme`. `useDocumentTheme` watches `html[data-theme]` via MutationObserver and always reflects the real current theme. Only `ThemeToggle` uses `useTheme` (it's the one calling `cycleTheme`).

**Header is `position: fixed`, not `position: sticky`.** Sticky + `backdrop-filter` breaks `transform` — the compositing layer ignores transform so only content moves, not the background. `--header-height: 61px` in `:root` is the single source of truth used by `appMain { padding-top }` and mobile drawer `top`.

**MobileNav must portal to `document.body`.** The header's `backdrop-filter` creates a stacking context that traps fixed children and renders their backgrounds transparent. Any modal/drawer/overlay inside the header must use `createPortal(el, document.body)`.

**iOS Safari scroll lock.** `overflow: hidden` on body does not prevent scroll on iOS. Use: save `scrollY`, set `body { position: fixed; top: -${y}px; width: 100% }`. On close, restore and `window.scrollTo(0, y)`.

**Hamburger state machine.** Two independent states: `open` (controls drawer + backdrop), `closing` (controls hamburger reverse animation only). `setOpen(false)` and `setClosing(true)` fire simultaneously on close — drawer and hamburger animate in parallel. `CLOSE_DURATION = { glitch: 280, ember: 420, cosmos: 520 }` clears `closing` precisely when each animation ends.

**Theme system.** Three themes: `glitch` (blue/pink), `ember` (amber/crimson), `cosmos` (cyan/lavender). All color tokens are oklch() CSS custom properties on `[data-theme]` selectors in `globals.css`. FOUC prevented via inline `<script>` in `layout.tsx` + `suppressHydrationWarning` on `<html>`.

---

## Coding Conventions

- **Commits:** Small and frequent. One logical change per commit.
- **Explicit over magic:** Readable over clever, always.
- **PR workflow:** Feature branch → `gh pr create` → merge → delete branch.
- **Testing:** Smoke test all endpoints before opening a PR.

---

## Current State

### Backend Test Suite — 130 passing
- All handlers tested: Create, Update, Delete, GetBySlug for both Projects and Technologies; Login
- All validators tested: CreateProject, CreateTechnology, UpdateProject, UpdateTechnology, LoginRequest
- **Known issue:** 3 tests commented out — in-memory DB name conflicts when the same method name exists in two test classes. Root cause: `DbContextFactory.Create(nameof(MethodName))` shares state across classes that have the same method name. Fix: prefix db names with the class name (e.g. `$"{nameof(UpdateProjectCommandHandlerTests)}_{nameof(Slug_IsTrimmedAndLowercased)}"`). Affected tests: `UpdateTechnologyCommandHandlerTests.Slug_IsTrimmedAndLowercased`, `UpdateProjectCommandHandlerTests.Slug_IsTrimmedAndLowercased`, `CreateProjectCommandHandlerTests.MixedValidAndInvalidTechnologyIds_ThrowsInvalidOperationException`.
- **Note:** `Discipline` field was added to Technology after the test suite was written. Validator tests for CreateTechnology and UpdateTechnology will need updating to include `Discipline` in test payloads.

### Technology — Discipline Field
`Technology` has a `Discipline` field (required, max 50 chars). Valid values enforced by FluentValidation:
`Frontend` | `Backend` | `Database` | `Cloud` | `DevOps` | `AI`

This is separate from `Category` (which describes the *type*: Language, Framework, Library, ORM, Tool, Cloud Service, CI/CD, AI Assistant, etc.). Discipline is the search/filter facet; Category is the display grouping.

### Admin UI
- `/admin` — landing page with links to Technologies and Projects
- `/admin/technologies` — full CRUD: list table, modal form (create/edit), inline delete confirm
- `/admin/projects` — full CRUD: list table, modal form with technology multi-select (checkbox grid), inline delete confirm

### Seeded Technologies (19 total, local DB)
Frontend: Next.js, React, TypeScript, CSS Modules, react-icons
Backend: C#, ASP.NET Core, FluentValidation, Serilog, Scalar
Database: Entity Framework Core, SQL Server
Cloud: Azure App Service, Azure Static Web Apps, Azure SQL, Azure Key Vault, Azure Managed Identity
DevOps: GitHub Actions
AI: Claude

### Still To Do (Frontend)
- [ ] Active nav link highlighting (current page underline)
- [ ] Headshot: actual photo (placeholder in use)
- [ ] Technologies page card layout — grid/grouping still needs design work
- [ ] Technology detail pages — currently sparse; could add more content sections

### Immediate Next
1. **Fix commented-out tests** — prefix db names with class name; also update technology validator tests to include `Discipline`
2. **AI Job Fit feature** — user pastes job post, Azure OpenAI responds citing portfolio projects. Streaming response to frontend.
3. **Roles + multi-user auth** — `role` claim in JWT, `[Authorize(Roles = "Admin")]`, Users table, Entra ID or B2C

### Completed
- **AuthContext refactor** — `AuthProvider` at admin layout level; single `checkAuth()` call eliminates per-page auth waterfall; `AdminGuard` handles redirect; admin pages fire `load()` on mount unconditionally
- **Filter system** — `ProjectFilterModal` (Projects page: cascading Discipline → Category → Technology drill-down; only technologyIds written to URL); Technologies page uses Link-based discipline pill bar (no modal, no JS, server component)
- **Automated EF migrations in CI/CD** — `api-deploy.yml` runs `dotnet ef database update` before the App Service deploy; temporarily opens Azure SQL firewall for the runner IP, cleans up with `if: always()`; uses `ASPNETCORE_ENVIRONMENT=Development` to bypass Key Vault; requires `AZURE_SQL_CONNECTION_STRING` secret and `AZURE_RESOURCE_GROUP` variable in GitHub
- **Card layout overhaul** — Projects page: 2-col grid, featured cards span full width with horizontal image+content layout, regular cards vertical with image on top, sorted featured-first, picsum fallback for missing images; GlassCard: `height: 100%` for equal-height grid rows; Technologies page: equal-height cards via flex layout, CategoryIcon replaces TechIcon on list (brand icons saved for detail pages), removed featured prop from list cards (was making 80% look hovered)
- **Icon system** — `TechIcon`: expanded with Azure services, GitHub Actions, Claude, FluentValidation, Serilog, Scalar, CSS Modules; `DisciplineIcon`: per-discipline icons (Brush/Server/Database/Cloud/GitBranch/Brain); `CategoryIcon`: per-category icons (Braces/Stack/Book/Database/Tool/Cloud/Infinity/Brain); detail pages show individual brand TechIcon in header

---

## Key Files

| File | Purpose |
|---|---|
| `Portfolio.Api/Program.cs` | All DI registration and middleware pipeline |
| `Portfolio.Api/Features/` | All vertical slice handlers |
| `Portfolio.Api/Filters/ValidationFilter.cs` | Generic pre-action validation filter |
| `Portfolio.Api/Common/Projections/` | EF LINQ projections (entity → DTO at query level) |
| `Portfolio.Api/Extensions/AuthenticationExtensions.cs` | JWT Bearer + cookie auth wiring |
| `portfolio-web/src/app/globals.css` | All design tokens (3 themes) + animation keyframes |
| `portfolio-web/src/app/layout.tsx` | Root layout: FOUC script, CommandPalette data fetch |
| `portfolio-web/src/hooks/useTheme.ts` | Writes `data-theme` + localStorage — only use in ThemeToggle |
| `portfolio-web/src/hooks/useDocumentTheme.ts` | Read-only theme via MutationObserver — use everywhere else |
| `portfolio-web/src/hooks/useScrollDirection.ts` | Returns `hidden: boolean` for header hide/reveal |
| `portfolio-web/src/app/components/MobileNav.tsx` | Portaled drawer, scroll lock, hamburger state machine |
| `portfolio-web/src/lib/api.ts` | Typed frontend API client — includes `TechnologyWriteDto`, `ProjectWriteDto`, and all mutation fns |
| `portfolio-web/src/context/AuthContext.tsx` | Shared auth state (AuthProvider + useAuthContext) — consumed by admin layout |
| `portfolio-web/src/app/admin/layout.tsx` | Admin layout — mounts AuthProvider, redirects unauthenticated users via AdminGuard |
| `portfolio-web/src/app/admin/technologies/page.tsx` | Admin CRUD for technologies — list table + modal form |
| `portfolio-web/src/app/admin/projects/page.tsx` | Admin CRUD for projects — list table + modal form with technology multi-select |
| `portfolio-web/src/app/components/ProjectFilterModal.tsx` | Cascading filter modal (Projects page) — Discipline → Category → Technology drill-down |
| `portfolio-web/src/app/components/FilterModal.module.css` | Shared styles for filter modals |
| `portfolio-web/src/app/components/DisciplineIcon.tsx` | Maps discipline name → Tabler icon (Frontend=Brush, Backend=Server, Database, Cloud, DevOps=GitBranch, AI=Brain) |
| `portfolio-web/src/app/components/CategoryIcon.tsx` | Maps category name → Tabler icon (Language=Braces, Framework=Stack, Library=Book, Tool, CI/CD=Infinity, etc.) |
| `portfolio-web/src/app/components/TechIcon.tsx` | Maps technology slug → brand icon — used on detail pages and project cards only |
| `ARCHITECTURE.md` | Full design decision explanations with reasoning |

---

## Documentation Maintenance

When completing a feature or architectural decision, update:
- **`CLAUDE.md`** — Current State, Immediate Next
- **`README.md`** — new endpoints, setup changes
- **`ARCHITECTURE.md`** — new patterns or significant decisions
- **`DEVLOG.md`** — append entry (local only, not source controlled)
