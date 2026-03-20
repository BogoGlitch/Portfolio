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
| Frontend | Next.js 16 (App Router), React 19, TypeScript |
| Styling | CSS Modules |

---

## Architecture Decisions

### Vertical Slice Architecture
Features live in `Features/` organized by operation, not by layer. Each feature folder contains its query/command record and its handler — nothing else.

```
Features/
  Projects/
    Queries/GetProjects/         ← GetProjectsQuery + GetProjectsQueryHandler
    Queries/GetProjectBySlug/
    Commands/CreateProject/      ← CreateProjectCommand + CreateProjectCommandHandler
    Commands/UpdateProject/
    Commands/DeleteProject/
  Technologies/
    (same structure)
```

**Why:** When a feature changes, you open one folder. You don't chase logic across Controllers → Services → Repositories.

### No MediatR (Yet)
Handlers are injected directly into controllers by concrete type. Explicit, readable, no indirection.

**When we add it:** When cross-cutting concerns (logging every handler, wrapping every command in a transaction) become painful to repeat across handlers. MediatR pipeline behaviors solve that problem — we'll add it when we feel the friction, not before.

### Explicit DI Registration
Everything in `Program.cs` is registered individually. No assembly scanning. You can read `Program.cs` and know exactly what the app can do.

### Problem Details (RFC 7807)
All error responses follow the standard shape: `{ type, title, status, detail, traceId }`. Configured via `AddProblemDetails()` + `UseExceptionHandler()` + `UseStatusCodePages()`. Custom `GlobalExceptionHandlingMiddleware` was removed.

### FluentValidation + ValidationFilter
`ValidationFilter<T>` is a generic action filter that runs before controller actions on POST/PUT. Validators mirror entity configuration constraints (same max lengths, required fields). Bad requests are rejected before the handler runs.

### CQRS Naming Convention
- **Queries** — read data, never mutate. Return a DTO or null.
- **Commands** — mutate data. Return the updated/created DTO, or bool for deletes.
- Slug uniqueness and technology ID validation live in command handlers, not validators (validators check shape; handlers check business rules).

---

## Coding Conventions

- **Commits:** Small and frequent. One logical change per commit. Never batch unrelated changes.
- **Explicit over magic:** If two approaches exist, choose the one you can read without knowing the framework internals.
- **Comments:** Only where the reason isn't obvious from the code itself. Handler files explain the *why* of non-obvious decisions (e.g. why we re-fetch after insert).
- **Error handling:** Handlers return `null` for not-found (controller produces 404). Handlers throw `InvalidOperationException` for business rule violations (Problem Details middleware produces 500 — we will refine this when we add proper domain exceptions).

---

## Current State

### Branches
- `main` — stable, all below merged in

### What's Done
- [x] EF Core entities: Project, Technology, ProjectTechnology (join)
- [x] LINQ projections for DTO mapping at query level
- [x] Problem Details error responses (RFC 7807)
- [x] FluentValidation with ValidationFilter<T>
- [x] Vertical slice handlers for Projects (2 queries, 3 commands)
- [x] Vertical slice handlers for Technologies (2 queries, 3 commands)
- [x] Controllers slimmed to HTTP-only, injecting handlers directly
- [x] Old ProjectService, TechnologyService, and interfaces deleted
- [x] Serilog structured logging (console + rolling file sinks, EF Core noise suppressed)

### Immediate Next
- [ ] Health checks endpoint

### Planned (not started)
- [ ] MediatR + pipeline behaviors (when cross-cutting pain is felt)
- [ ] Authentication / authorization
- [ ] AI "Job Fit" feature — user pastes job post, AI responds citing portfolio projects/technologies
- [ ] Azure: App Service deployment, Key Vault, CI/CD via GitHub Actions
- [ ] Azure Application Insights (tied to Serilog)
- [ ] Frontend polish: streaming AI responses, dark mode, animations

---

## Key Files

| File | Purpose |
|---|---|
| `Portfolio.Api/Program.cs` | All DI registration and middleware pipeline |
| `Portfolio.Api/Features/` | All vertical slice handlers |
| `Portfolio.Api/Filters/ValidationFilter.cs` | Generic pre-action validation |
| `Portfolio.Api/Validators/` | FluentValidation rules per DTO |
| `Portfolio.Api/Common/Projections/` | EF LINQ projections (entity → DTO) |
| `Portfolio.Api/Extensions/SerilogConfiguration.cs` | Serilog setup (kept out of Program.cs deliberately) |
| `portfolio-web/src/app/` | Next.js App Router pages and components |
| `portfolio-web/src/lib/api.ts` | Frontend API client |
