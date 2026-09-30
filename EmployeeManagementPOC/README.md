# Employee Management

ASP.NET Core 10 Web API (Clean Architecture + DDD) with an Angular 20 SPA.

```
EmployeeManagementPOC/
├── src/
│   ├── EmployeeManagement.Domain          # Aggregates, value objects, invariants, repository ports
│   ├── EmployeeManagement.Application     # Use cases, DTOs, validators, read-side query ports
│   ├── EmployeeManagement.Infrastructure  # EF Core (writes), Dapper (reads), Identity/JWT, caching
│   └── EmployeeManagement.Api             # Versioned controllers, filters, error handling, OpenAPI
└── client/                                # Angular 20 SPA (standalone, zoneless, signals)
```

Dependencies point inwards only: `Api → Infrastructure → Application → Domain`.

## Backend

| Concern | Where | Notes |
|---|---|---|
| Domain model | `Domain/Employees`, `Domain/Departments` | Private setters, factory methods (`Employee.Hire`, `Department.Create`), `Email` value object. Aggregates reference each other by id only. |
| Writes | `Infrastructure/Persistence` | EF Core repositories + `IUnitOfWork` (the `DbContext`). |
| Reads | `Infrastructure/Queries` | Dapper projections straight to response DTOs (CQRS-lite); dashboard is a single round-trip. |
| Validation | `Application/**/…Validator.cs` + `Api/Filters/ValidationFilter` | FluentValidation, run by a global action filter → `400` validation problem. |
| Error handling | `Api/ErrorHandling/GlobalExceptionHandler` | `IExceptionHandler` → RFC 9457 problem details with `code` and `traceId`. `NotFound→404`, `Conflict→409`, `DomainException→422`, auth failure `→401`, anything else an opaque `500`. |
| API versioning | `Api/Versioning` | `Asp.Versioning`; URL segment (`/api/v1/...`) or `X-Api-Version` header. Dashboard has v1 (deprecated, advertised via `api-deprecated-versions`) and v2. Swagger has one document per version. |
| Auth | `Infrastructure/Identity`, `Api/Authorization/Policies` | ASP.NET Identity for users/lockout, JWT bearer tokens, role-based policies. |
| Caching | `Infrastructure/Caching` | Cache-aside over `IDistributedCache` (Redis, or in-memory when no Redis is configured). If the cache fails, requests still go to the database. |

The EF model maps to the **same schema** as the original MVC app (the existing migrations were kept), so existing databases work unchanged.

## Frontend

| Concern | Where |
|---|---|
| HTTP interceptors | `core/http/` — `authInterceptor` (bearer token, own API only), `errorInterceptor` (401 → login, 403 → access denied, otherwise a toast from problem details), `loadingInterceptor` (global progress bar). Per-request opt-outs via `HttpContext` (`SKIP_ERROR_TOAST`, `SKIP_LOADING`). |
| Global error handler | `core/errors/global-error-handler.ts` |
| Guards | `core/auth/auth.guards.ts` — `authGuard`, `guestGuard`, `roleGuard(...)` |
| Pipes ("filters") | `shared/pipes/` — `filterBy` (client-side list filter), `initials`, `pluralize` |
| Employee list filters | `features/employees/` — search (debounced) with autocomplete, department, status, sort; state lives in the URL query string |
| Autocomplete | Suggestions under the search box come from `GET /api/v1/employees/suggestions`, served by an in-memory **Trie** from [`DataStructuresAlgorithmsPOC`](../DataStructuresAlgorithmsPOC/README.md) |
| Server-side form errors | `shared/forms/server-errors.ts` maps `400` field errors and business error codes (e.g. duplicate email) onto form controls |

## Running locally

Prerequisites: .NET 10 SDK, Node 22, and a local SQL Server default instance. Development connects with Windows authentication (`Server=localhost;Trusted_Connection=True`). If your SQL Server is elsewhere, override it without editing the file:

```bash
dotnet user-secrets --project src/EmployeeManagement.Api set ConnectionStrings:DefaultConnection "<your connection string>"
```

```bash
# API: http://localhost:5297 (Swagger at /swagger)
dotnet run --project src/EmployeeManagement.Api

# SPA: http://localhost:4200 (proxies /api to the API)
cd client && npm install && npm start
```

In Development the API applies migrations and seeds one demo user per role:

| Role | Email | Password |
|---|---|---|
| Admin | admin.user@poc.local | Admin@12345 |
| HR | hr.user@poc.local | Hr@12345 |
| Manager | manager.user@poc.local | Manager@12345 |
| Employee | employee.user@poc.local | Employee@12345 |

### Docker

```bash
docker compose up --build   # http://localhost:5001 (run from this folder)
```

The image builds the Angular app and serves it from the API's `wwwroot`, so the SPA and API share one origin. The build context is the repository root because the API references `../DataStructuresAlgorithmsPOC`; to build the image directly, run `docker build -f EmployeeManagementPOC/Dockerfile .` from the root.

## Configuration

| Key | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | SQL Server (required) |
| `ConnectionStrings:Redis` | Optional; in-memory cache is used when empty |
| `Jwt:SigningKey` | Required, at least 32 chars. The app fails at startup when it's missing. Supply it from a secret store outside development. |
| `Database:ApplyMigrationsOnStartup` | Dev/demo convenience |
| `Database:SeedDemoUsers` | Dev/demo only |
