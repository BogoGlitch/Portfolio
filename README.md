# Portfolio

A personal portfolio site built to demonstrate enterprise-level full-stack engineering. The architecture is intentionally thorough — every decision can be explained and defended.

**Dual purpose:** Personal website + living proof of enterprise capability for potential employers and clients.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Web API, .NET 10 |
| ORM | Entity Framework Core 10, SQL Server |
| Validation | FluentValidation 11 |
| Logging | Serilog 9 |
| Auth | JWT Bearer, HttpOnly cookies |
| Frontend | Next.js 16 (App Router), React 19, TypeScript |
| Styling | CSS Modules |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or Docker)
- Node.js 20+

---

## Running Locally

### Backend

```bash
cd Portfolio.Api
dotnet run --launch-profile https
```

API runs at `https://localhost:7166`. Swagger UI at `https://localhost:7166/swagger`.

### Frontend

```bash
cd portfolio-web
npm install
npm run dev
```

Frontend runs at `http://localhost:3000`.

---

## Environment Configuration

Copy `appsettings.Development.json` and fill in:

```json
{
  "Jwt": {
    "Key": "<min 32 character secret>",
    "Issuer": "portfolio-api",
    "Audience": "portfolio-web",
    "ExpiryHours": "8"
  },
  "AdminCredentials": {
    "Username": "<your username>",
    "Password": "<your password>"
  }
}
```

The `Jwt:Key` and `AdminCredentials` values are intentionally blank in `appsettings.json`. Never commit real secrets.

---

## API Endpoints

### Auth
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/login` | Public | Authenticate and receive JWT cookie |
| POST | `/api/auth/logout` | Public | Clear JWT cookie |

### Projects
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/projects` | Public | List all projects |
| GET | `/api/projects/{slug}` | Public | Get project by slug |
| POST | `/api/projects` | Required | Create project |
| PUT | `/api/projects/{id}` | Required | Update project |
| DELETE | `/api/projects/{id}` | Required | Delete project |

### Technologies
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/technologies` | Public | List all technologies |
| GET | `/api/technologies/{slug}` | Public | Get technology by slug |
| POST | `/api/technologies` | Required | Create technology |
| PUT | `/api/technologies/{id}` | Required | Update technology |
| DELETE | `/api/technologies/{id}` | Required | Delete technology |

### Health
| Method | Endpoint | Description |
|---|---|---|
| GET | `/health` | Full check: process + database |
| GET | `/health/live` | Liveness only: process up |

---

## Project Structure

```
Portfolio.Api/
  Controllers/        HTTP layer only — no business logic
  Features/           Vertical slice handlers (queries + commands)
  Dtos/               Request and response shapes
  Validators/         FluentValidation rules
  Filters/            Generic ValidationFilter<T>
  Services/           ITokenService / JwtTokenService
  Extensions/         SerilogConfiguration, HealthCheckExtensions, AuthenticationExtensions
  Common/Projections/ EF LINQ projections (entity → DTO at query level)

portfolio-web/
  src/app/            Next.js App Router pages and components
  src/hooks/          useTheme, useDocumentTheme, useScrollDirection, useTypewriter, useIntersectionObserver
  src/lib/api.ts      Typed API client
```
