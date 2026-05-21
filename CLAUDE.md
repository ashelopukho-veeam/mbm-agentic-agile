# Agent Context — Marketing Budget Management (MBM)

<!--
  This file gives AI coding agents (Claude, Copilot, Cursor, etc.) the
  project-specific context they need to produce output that fits MBM
  conventions from the first interaction.

  Keep it updated. An outdated context file is worse than none, because
  agents will follow stale instructions confidently.
-->

---

## Project Purpose

Marketing Budget Management (MBM) is an internal tool for managing marketing budgets across the organization. Marketing teams use it to plan, track, and control budget allocations across cost centers, vendors, paid media, and incremental funds, replacing manual spreadsheet workflows with a structured, integrated platform.

The system is a single product with two deployable components living in one repository:

- **Frontend (`mbt-spa`)** — React 18 + TypeScript single-page application.
- **Backend (`mbt.webapi`)** — C# / .NET 8 Web API backed by MongoDB.

**Primary languages:** TypeScript (frontend), C# (backend)
**Frameworks:** React 18 + Vite (frontend), ASP.NET Core 8 / Ardalis.ApiEndpoints (backend)
**Data store:** MongoDB (via `MongoDB.Driver` + `MongoDBMigrations`)
**Authentication:** Microsoft Entra ID (Azure AD) — `Microsoft.Identity.Web` on backend, `@azure/msal-react` on frontend
**Background work:** Quartz.NET (scheduled jobs), WorkflowCore (long-running workflows)

---

## Repository Structure

```
mbm-agentic-agile/
├── src/
│   ├── backend/
│   │   └── mbt.webapi/             # ASP.NET Core 8 Web API
│   │       ├── Endpoints/          # Ardalis.ApiEndpoints route handlers (per domain folder)
│   │       ├── UseCases/           # MediatR request/handler pairs (application layer)
│   │       ├── Services/           # Business logic / domain services
│   │       ├── Repositories/       # MongoDB data access
│   │       ├── Domain/             # Entities, value objects, domain types
│   │       ├── Shared/             # DTOs and cross-cutting contracts
│   │       ├── Configuration/      # appsettings binding, options classes
│   │       ├── Middleware/         # ASP.NET middleware (ProblemDetails, auth, etc.)
│   │       ├── Extensions/         # DI / service collection extension methods
│   │       ├── Migrations/         # MongoDBMigrations versioned migrations + seed data
│   │       ├── Jobs/               # Quartz scheduled jobs
│   │       ├── WF/                 # WorkflowCore workflow definitions and steps
│   │       ├── BuiltIn/            # Built-in templates (mail, etc.)
│   │       ├── Utils/              # Shared utility helpers
│   │       ├── Properties/         # launchSettings.json
│   │       ├── Program.cs          # App composition root
│   │       ├── MappingProfile.cs   # AutoMapper profile
│   │       ├── Resx.cs             # String resources
│   │       ├── nlog.config         # NLog configuration
│   │       ├── appsettings*.json
│   │       ├── Dockerfile
│   │       └── mbt.webapi.csproj
│   └── frontend/
│       └── mbt-spa/                # React 18 + Vite SPA
│           ├── src/
│           │   ├── pages/          # Route-level views
│           │   ├── layouts/        # Page layouts / shells
│           │   ├── components/     # Shared UI (Radix + Tailwind, shadcn-style)
│           │   ├── controls/       # Form/UI primitives
│           │   ├── api/            # axios-based API client modules
│           │   ├── hooks/          # Reusable React hooks
│           │   ├── router/         # react-router-dom configuration
│           │   ├── dto/            # DTOs mirroring backend contracts
│           │   ├── types/          # Shared TS types
│           │   ├── utils/
│           │   ├── assets/
│           │   ├── e2e/            # End-to-end tests
│           │   ├── App.tsx, main.tsx
│           │   ├── AppContext.tsx, AuthContextProvider.tsx, useAuth.tsx
│           │   ├── authConfig.ts, msalInstance.ts, policies.ts
│           │   └── queryClient.tsx
│           ├── public/
│           ├── builds/
│           ├── package.json, vite.config.ts, tsconfig.json
│           ├── tailwind.config.js, components.json (shadcn)
│           ├── .eslintrc.cjs, .prettierrc.json
│           └── index.html
├── docs/                           # Project documentation
├── .github/                        # Workflows and issue/PR templates
├── CLAUDE.md                       # This file
├── STYLE.md                        # Code and documentation style guide
├── CONTRIBUTING.md
├── MANIFESTO.md                    # Agentic-Agile Manifesto
├── SECURITY.md
├── README.md
├── LICENSE
└── mcp.json                        # Example MCP server configuration
```

> **Note:** Frontend and backend were previously two separate repositories and were merged into this monorepo. Some legacy paths and configs (e.g. `mbt-spa/.gitlab-ci.yml`, `mbt-spa/README.md`) may still reference the old layout — treat them as historical until consciously migrated.

---

## Development Process

### Workflow

```
Plan → Issue → Implement → Review → Merge → Docs
```

| Phase         | Description                                                                                              |
| ------------- | -------------------------------------------------------------------------------------------------------- |
| **Plan**      | Define what to build. Identify scope, dependencies, and file ownership (which files the story owns).     |
| **Issue**     | Create a GitHub Issue with structured scope, acceptance criteria, and negative constraints.              |
| **Implement** | Build the feature on a feature branch. Follow conventions below. Add/update tests.                       |
| **Review**    | Submit a PR. Every PR receives review that checks correctness, test coverage, and convention compliance. |
| **Merge**     | Merge to `main` after review approval.                                                                   |
| **Docs**      | Update documentation affected by the change. Close the issue.                                            |

### Branch Strategy

- **`main`** — stable, production-ready code.
- **Feature branches** — one per issue, branched from `main`.
  - Naming: `[type]/[issue-id]-[short-description]` (e.g., `feat/123-budget-transfer`, `fix/456-null-vendor-name`).
- **Merge flow:** feature → `main` via reviewed PR (squash or merge commit per project policy).

### Review Standards

- Every PR receives at least one review.
- Review findings must reach a terminal state: `fixed`, `accepted` (with justification), or `deferred` (tracked as a follow-up issue).
- Agents may be used for implementation, but review remains a human responsibility until trust is established through measured outcomes.

### Issues-First Rule

- **Every work request must be captured as a GitHub Issue before implementation begins.** Do not start coding without a tracking issue.
- **The originating human prompt must be preserved in the issue** — either in the "Originating Prompt" section of the agentic-story template or as a comment. This enables retrospective evaluation of prompt quality (D4: Partnership Efficiency).
- If a human prompt contains multiple work items, create separate issues for each.

---

## Coding Conventions

### Backend (.NET 8 / C#)

- **Endpoints layer (Ardalis.ApiEndpoints):** one endpoint class per HTTP operation, grouped under `Endpoints/<DomainName>/`. Endpoints stay thin — they parse the request, dispatch to a `MediatR` request, and return the result. No business logic in endpoints.
- **UseCases layer (MediatR):** application logic lives in `UseCases/<Feature>/` as `Request` + `Handler` pairs. Handlers orchestrate services and repositories.
- **Services layer:** domain/business logic. Stateless. Registered via DI; prefer constructor injection. `Scrutor` is used for assembly scanning — follow the existing registration convention in `Extensions/`.
- **Repositories layer:** all MongoDB access goes through a repository in `Repositories/`. Do not call `IMongoCollection<T>` directly from endpoints/services/handlers.
- **DTOs vs. domain entities:** never return MongoDB entities directly from endpoints. Map to DTOs (in `Shared/` or per-feature DTO files) via the `MappingProfile` (AutoMapper).
- **Validation:** input validation uses `FluentValidation` validators. Validators are auto-registered via DI; place them next to the request/DTO they validate. Do not duplicate validation logic in handlers.
- **Routing constants:** keep route templates in `Endpoints/Routes.cs` rather than hardcoded inline.
- **Async by default:** all I/O methods are `async`, return `Task`/`Task<T>`, and accept a `CancellationToken` parameter wired through from the endpoint.
- **Nullable reference types:** respect the project's nullable annotations. Do not silence warnings with `!` unless invariant-justified with a comment.
- **No magic strings/numbers for domain concepts:** use enums, constants in `Resx.cs`, or per-feature constant classes.
- **API response shape:** errors flow through `Hellang.Middleware.ProblemDetails` and return RFC 7807 `ProblemDetails`. Do not invent ad-hoc error envelopes.
- **Do not close, implement, or modify issues labeled `sample`.** These are onboarding references (`[SAMPLE 1]`, `[SAMPLE 2]`), not real work items.

### Frontend (React 18 + TypeScript)

- **TypeScript strictness:** components, hooks, and API modules are fully typed. No `any` unless explicitly justified — prefer `unknown` + narrowing.
- **Components:** function components only. Co-locate component + styles + tests in the same folder. Use Radix primitives via the shadcn-style wrappers in `components/ui/` rather than building one-off primitives.
- **Styling:** Tailwind CSS. Compose class names via `clsx` / `tailwind-merge` (`cn` helper). Avoid inline `style={}` except for dynamic values that cannot be expressed in Tailwind.
- **Forms:** `react-hook-form` + `zod` resolvers. Define the zod schema next to the form and infer the form type from it (`z.infer<typeof schema>`).
- **Data fetching:** all server state goes through `@tanstack/react-query`. API call functions live in `src/api/` and return typed promises. Do not call `axios` directly from components — go through `src/api/`.
- **Tables:** use `@tanstack/react-table` for any non-trivial tabular UI; align with existing column-definition patterns.
- **Routing:** `react-router-dom` v6. Route definitions live in `src/router/`.
- **Auth:** Microsoft Entra ID via `@azure/msal-react`. Token acquisition and auth state go through `AuthContextProvider` / `useAuth` — do not call MSAL APIs directly from feature code.
- **Path imports:** prefer the configured TS path aliases (see `tsconfig.json`) over long relative paths.

### Error Handling

**Backend:**
- Throw typed exceptions from the domain/services layer; let the global `ProblemDetails` middleware translate them into HTTP responses with appropriate status codes.
- Never swallow exceptions silently. If an exception is intentionally absorbed (e.g., best-effort cleanup), log it at `Warn` with context.
- Validation failures surface as `400 Bad Request` via FluentValidation → ProblemDetails. Do not hand-roll `400`s in handlers.
- Log errors at the boundary (middleware / endpoint), not at every layer in the stack.

**Frontend:**
- API errors are surfaced through `react-query`'s `error` state and rendered via `sonner` toasts or inline form errors — do not swallow them.
- Validate user input client-side with zod schemas, but never rely on client-side validation alone; the backend re-validates.

### Logging

- **Backend:** structured logging via `NLog` (see `nlog.config`). Use `ILogger<T>` with message templates (`logger.LogInformation("Loaded budget {BudgetId}", id)`) — never string interpolation. Include identifiers (request id, user id, budget id) as structured properties, not embedded in the message string.
- **Log levels:** `Debug` for development diagnostics, `Information` for operational events, `Warning` for recoverable anomalies, `Error` for failures requiring attention.
- **Frontend:** use `console.error` only for unexpected failures; for user-visible errors, surface via toast/form. Do not log PII or tokens to the console.

### Security

- Never commit secrets, tokens, or credentials. Use environment variables / appsettings overrides / `.env` files excluded from VCS.
- Validate and sanitize all external input on the backend, regardless of client-side validation.
- All endpoints require authentication unless explicitly marked anonymous; authorization policies are defined in `policies.ts` (frontend) and configured in `Program.cs` / endpoint attributes (backend).
- Never log tokens, full request bodies containing PII, or authorization headers.
- Treat CSV/file uploads as untrusted: validate MIME, size, and parsed content shape before persisting.

---

## Testing Strategy

### Backend

- **Test framework:** **xUnit**.
- **Assertions:** `FluentAssertions` (preferred) or built-in xUnit asserts for trivial cases.
- **Mocking:** `Moq` or `NSubstitute` — pick the one already used in the test project; do not mix within a single project.
- **Coverage tool:** `coverlet` via `dotnet test --collect:"XPlat Code Coverage"`.
- **Architecture tests:** `NetArchTest.Rules` is already referenced — use it to enforce layer dependencies (e.g., `Endpoints` may depend on `UseCases`, but not vice versa).

### Frontend

- **Test framework:** **Vitest** (Vite-native; reuses the project's Vite config).
- **DOM testing:** `@testing-library/react` + `@testing-library/user-event` + `@testing-library/jest-dom`.
- **Mocking:** Vitest's built-in `vi.mock` / `vi.fn`. For network mocking prefer `msw` over hand-rolled axios mocks.
- **E2E:** the `src/e2e/` directory exists and is reserved for end-to-end tests (Playwright recommended).

### Test Organization

- **Backend:** test projects live under `src/backend/` (e.g., `mbt.webapi.tests/`) and mirror the production project's namespace structure.
- **Frontend:** unit/component tests sit next to the file under test as `Foo.test.ts(x)` or `Foo.spec.ts(x)`. End-to-end tests live in `src/e2e/`.

### Test Naming

- **Backend (xUnit):** test class matches the system under test (`BudgetServiceTests`); test method names describe behavior: `Approve_WhenBudgetIsDraft_TransitionsToApproved`.
- **Frontend (Vitest):** `describe` blocks match the component/hook name; test names describe behavior: `should disable submit when form is invalid`.

### Coverage Expectations

- All new public methods/components ship with at least one test.
- Target ≥ 70% line coverage on changed files; critical paths (auth, money calculations, budget transitions) require ≥ 90%.
- A PR that lowers coverage on a touched file requires explicit justification in the PR description.

### Running Tests

```bash
# Backend
dotnet test src/backend/mbt.webapi.sln                            # all tests
dotnet test --collect:"XPlat Code Coverage"                       # with coverage
dotnet test --filter "FullyQualifiedName~BudgetServiceTests"      # specific class

# Frontend (run from src/frontend/mbt-spa)
npm test                                                          # all unit tests
npm test -- --coverage                                            # with coverage
npm test -- src/components/Budget/BudgetForm.test.tsx             # specific file
```

> **Note:** test scripts (`npm test`, backend test project) are not yet wired up in the merged repo. Add them in an early wave before relying on the commands above.

---

## Documentation Maintenance

| Document                       | Update When                                                |
| ------------------------------ | ---------------------------------------------------------- |
| `README.md`                    | Project scope, setup, or usage changes                     |
| `CLAUDE.md`                    | Process, conventions, structure, or tooling changes        |
| `STYLE.md`                     | Style conventions change                                   |
| `CONTRIBUTING.md`              | Contribution process changes                               |
| API docs (Swagger/OpenAPI)     | Endpoints added, modified, or removed                      |
| `src/backend/mbt.webapi/Migrations/` | MongoDB schema changes (add a new migration, never edit a merged one) |
| `docs/`                        | Architecture, decisions, or onboarding material changes    |

---

## Parallelization Rules

### File Ownership

Every story explicitly declares which files it owns. No two stories in the same wave (parallel execution batch) may touch the same file.

For MBM specifically:
- A story that changes a backend contract (DTO in `Shared/`, route in `Routes.cs`, validator) **and** its frontend consumer must either own both files, or split into two stories with the contract change landing first.
- Shared configuration files (`Program.cs`, `MappingProfile.cs`, `src/router/`, `queryClient.tsx`, `tailwind.config.js`) are conflict-prone — assign them to a single story per wave.

### Wave Structure

```
Wave 1: [Story A: owns src/backend/.../Endpoints/Budgets/] [Story B: owns src/frontend/.../pages/Vendors/]
         ↓ review gate ↓
Wave 2: [Story C: depends on Wave 1 contracts] [Story D: owns docs/]
         ↓ review gate ↓
Wave 3: ...
```

### Dependency Gates

- A story cannot start until all stories it depends on are complete and merged.
- Review gates between waves ensure quality before dependent work begins.
- Cross-cutting concerns (shared DTOs, route constants, auth policies) are resolved in earlier waves.

### Conflict Prevention

- Two stories that must touch the same file go in sequential waves.
- Shared interfaces / DTOs / route constants are introduced by a dedicated story that completes before stories consuming them.

---

## Validation Gates

Before a PR is considered ready for merge:

- [ ] **Tests pass** — all existing and new tests pass (`dotnet test`, `npm test`).
- [ ] **Build clean** — `dotnet build` succeeds with no new warnings; `npm run build` succeeds.
- [ ] **Linter clean** — `npm run lint` has zero warnings (`--max-warnings 0` is enforced).
- [ ] **Types check** — TypeScript `tsc` passes; C# nullable warnings are addressed.
- [ ] **Documentation updated** — affected docs (README, CLAUDE.md, API docs) are current.
- [ ] **No unrelated changes** — PR is scoped to the issue.
- [ ] **Acceptance criteria met** — all criteria from the issue are satisfied.
- [ ] **Negative constraints respected** — nothing out-of-scope was modified.
- [ ] **Review complete** — at least one review with all findings resolved.

---

## Build and Run

### Backend (`src/backend/mbt.webapi`)

```bash
# Restore + build
dotnet restore src/backend/mbt.webapi/mbt.webapi.csproj
dotnet build   src/backend/mbt.webapi/mbt.webapi.csproj

# Run locally (uses appsettings.Development.json)
dotnet run --project src/backend/mbt.webapi/mbt.webapi.csproj

# Tests
dotnet test

# Docker image
docker build -t mbt.webapi -f src/backend/mbt.webapi/Dockerfile .
```

Requires .NET 8 SDK and a reachable MongoDB instance (connection string in `appsettings.Development.json` or env override).

### Frontend (`src/frontend/mbt-spa`)

```bash
# From src/frontend/mbt-spa
npm install

# Dev server (Vite, hot reload)
npm run dev

# Production build (tsc + vite build)
npm run build

# Lint
npm run lint

# Preview production build
npm run preview
```

Requires Node.js (LTS, see `package.json` engines if pinned) and access to the backend API URL configured in the Vite proxy / env.

---

## External Integrations

| Integration              | Purpose                                                   | Where configured                                                  |
| ------------------------ | --------------------------------------------------------- | ----------------------------------------------------------------- |
| **Microsoft Entra ID**   | Authentication for users (SPA) and API (JWT bearer)       | Backend: `Program.cs` + `appsettings.json`. Frontend: `authConfig.ts`, `msalInstance.ts`. |
| **MongoDB**              | Primary data store; schema migrations via `MongoDBMigrations` | `appsettings.json` (connection), `Migrations/`                    |
| **Microsoft Graph**      | Directory lookups (users, groups)                         | `Microsoft.Identity.Web.GraphServiceClient` in `Program.cs`       |
| **NLog → Microsoft Teams** | Error alerts to a Teams channel                         | `nlog.config`                                                     |
| **Prometheus**           | Metrics endpoint for ops                                  | `prometheus-net.AspNetCore` in `Program.cs`                       |
| **Quartz.NET**           | Scheduled background jobs                                 | `Jobs/`                                                           |
| **WorkflowCore**         | Long-running budget / approval workflows                  | `WF/`                                                             |
| **MailKit (SMTP)**       | Outbound notification email                               | `appsettings.json`, templates in `BuiltIn/MailTemplates/`         |
