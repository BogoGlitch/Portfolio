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
| Styling | CSS Modules |

---

## Architecture Decisions

### Vertical Slice Architecture
Features live in `Features/` organized by operation, not by layer. Each feature folder contains its query/command record and its handler — nothing else.

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
```

**Why:** When a feature changes, you open one folder. No chasing logic across Controllers → Services → Repositories.

### No MediatR (Yet — Intentional)
Handlers are injected directly into controllers by concrete type. Explicit, readable, no indirection. The user is learning *why* MediatR exists by building without it first.

**When we add it:** When cross-cutting concerns (auto-logging every handler, wrapping commands in transactions) become painful to repeat. MediatR pipeline behaviors solve that — we add it when we feel the friction, not before. This is the agreed approach.

### Explicit DI Registration
Everything in `Program.cs` is registered individually. No assembly scanning. You can read `Program.cs` and know exactly what the app can do.

### Problem Details (RFC 7807)
All error responses: `{ type, title, status, detail, traceId }`. Configured via `AddProblemDetails()` + `UseExceptionHandler()` + `UseStatusCodePages()`.

### FluentValidation + ValidationFilter
`ValidationFilter<T>` is a generic action filter that runs before POST/PUT actions. Validators mirror entity config constraints. Bad requests are rejected before the handler runs.

### CQRS Naming Convention
- **Queries** — read only, return DTO or null
- **Commands** — mutate data, return DTO or bool
- Shape validation → validators. Business rule validation (slug uniqueness, foreign key checks) → command handlers.

### Serilog
Configured in `Extensions/SerilogConfiguration.cs` (kept out of Program.cs deliberately). Two sinks: console (dev) and rolling file under `logs/` (14-day retention). EF Core and ASP.NET Core framework logs suppressed to Warning — only app-level logs show at Information. `logs/` and `.claude/` are in `.gitignore`.

### Health Checks
- `GET /health` — full check: process + database (EF Core DbContext query)
- `GET /health/live` — liveness only: process up, no dependency checks
- Both return structured JSON. Configured in `Extensions/HealthCheckExtensions.cs`.

---

## Coding Conventions

- **Commits:** Small and frequent. One logical change per commit.
- **Explicit over magic:** Always choose the readable approach over the clever one.
- **PR workflow:** Every feature on its own branch → PR via `gh pr create` → merge → delete branch. `gh` is in Git Bash PATH via `~/.bashrc`.
- **Comments:** Only where the reason isn't obvious. Handlers explain *why* for non-obvious decisions.
- **Error handling:** Handlers return `null` for not-found (controller → 404). Handlers throw `InvalidOperationException` for business rule violations (→ 500 via Problem Details). Will refine with domain exceptions later.
- **Testing:** Run and smoke test after every branch before opening a PR. All endpoints verified manually.

---

## Current State

### Branches
- `main` — stable, everything below is merged

### What's Done (Backend)
- [x] EF Core entities: Project, Technology, ProjectTechnology (join table)
- [x] LINQ projections for DTO mapping at query level (no in-memory mapping)
- [x] Problem Details (RFC 7807) error responses
- [x] FluentValidation with generic ValidationFilter<T>
- [x] Vertical slice handlers: Projects (2 queries, 3 commands)
- [x] Vertical slice handlers: Technologies (2 queries, 3 commands)
- [x] Controllers: HTTP-only, inject handlers directly by concrete type
- [x] Serilog: structured logging, console + rolling file, EF noise suppressed
- [x] Health checks: /health and /health/live with JSON responses

### What's Done (Frontend)
- [x] Next.js 16 App Router scaffold
- [x] Pages: Home, Projects (list + [slug]), Technologies (list + [slug])
- [x] Shared Header, Footer, PageLayout components
- [x] API client in src/lib/api.ts

### Immediate Next
1. **MediatR + pipeline behaviors** — the natural next backend step. User understands *why* it exists. Add it, then show auto-logging as the first pipeline behavior.
2. **Authentication** — after MediatR
3. **Azure deployment** — App Service, Key Vault, CI/CD via GitHub Actions
4. **AI Job Fit feature** — user pastes job post, Claude/Azure OpenAI responds citing portfolio projects/technologies from DB. Streaming response to frontend.

---

## Key Files

| File | Purpose |
|---|---|
| `Portfolio.Api/Program.cs` | All DI registration and middleware pipeline — kept as a table of contents |
| `Portfolio.Api/Features/` | All vertical slice handlers (queries + commands) |
| `Portfolio.Api/Filters/ValidationFilter.cs` | Generic pre-action validation filter |
| `Portfolio.Api/Validators/` | FluentValidation rules per DTO |
| `Portfolio.Api/Common/Projections/` | EF LINQ projections (entity → DTO at DB level) |
| `Portfolio.Api/Extensions/SerilogConfiguration.cs` | Serilog setup |
| `Portfolio.Api/Extensions/HealthCheckExtensions.cs` | Health endpoint mapping |
| `portfolio-web/src/app/` | Next.js App Router pages and components |
| `portfolio-web/src/lib/api.ts` | Frontend API client |
