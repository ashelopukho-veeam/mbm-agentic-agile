# Copilot Instructions

> This file provides GitHub Copilot with project-specific context. For the full development process, see `CLAUDE.md` in the repository root.

## Project Description

Marketing Budget Management (MBM) is an internal tool for planning, tracking, and controlling marketing budgets across cost centers, vendors, paid media, and incremental funds. The system is a monorepo with a React 18 + TypeScript SPA (`mbt-spa`) and an ASP.NET Core 8 Web API (`mbt.webapi`) backed by MongoDB. Authentication is Microsoft Entra ID (`@azure/msal-react` on the frontend, `Microsoft.Identity.Web` on the backend).

## Coding Style

- **Languages:** TypeScript (frontend), C# / .NET 8 (backend).
- **Frontend formatting:** Prettier + ESLint (`--max-warnings 0`); Tailwind CSS for styles via the `cn` helper (`clsx` + `tailwind-merge`).
- **Backend formatting:** standard .NET conventions; respect project nullable annotations (no `!` suppression without justification).
- **Naming:** TS — `camelCase` vars/functions, `PascalCase` components/types. C# — `PascalCase` types/methods, `_camelCase` private fields, `camelCase` parameters/locals.
- **Imports:** TS — prefer configured `tsconfig.json` path aliases over long relative paths. C# — group `System.*`, third-party, then project namespaces.
- **Error handling:** backend throws typed exceptions and lets `Hellang.Middleware.ProblemDetails` translate them to RFC 7807 responses — do not invent ad-hoc error envelopes. Frontend surfaces API errors via `react-query` `error` state and `sonner` toasts or inline form errors. Never swallow exceptions silently.

## Testing Approach

- **Backend framework:** xUnit with `FluentAssertions`; mocks via `Moq` or `NSubstitute` (do not mix in one project). Coverage via `coverlet`. Layer rules enforced with `NetArchTest.Rules`.
- **Frontend framework:** Vitest + `@testing-library/react` / `user-event` / `jest-dom`. Network mocks via `msw`. E2E in `src/frontend/mbt-spa/src/e2e/` (Playwright recommended).
- **Location:** backend tests under `src/backend/` mirroring production namespaces (e.g., `mbt.webapi.tests/`). Frontend unit/component tests sit next to the file under test as `Foo.test.ts(x)` / `Foo.spec.ts(x)`.
- **Naming:** xUnit — `Method_WhenCondition_ExpectedResult`. Vitest — `describe` matches the unit; test names describe behavior (`should disable submit when form is invalid`).
- **Coverage expectations:** every new public method/component ships with at least one test. Target ≥ 70% line coverage on changed files; ≥ 90% for critical paths (auth, money calculations, budget transitions).

## File Structure

```
mbm-agentic-agile/
├── src/
│   ├── backend/
│   │   └── mbt.webapi/             # ASP.NET Core 8 Web API
│   │       ├── Endpoints/          # Ardalis.ApiEndpoints handlers (per domain)
│   │       ├── UseCases/           # MediatR Request + Handler pairs
│   │       ├── Services/           # Domain/business logic
│   │       ├── Repositories/       # MongoDB data access (only place that touches IMongoCollection<T>)
│   │       ├── Domain/             # Entities, value objects
│   │       ├── Shared/             # DTOs and cross-cutting contracts
│   │       ├── Configuration/      # Options classes, appsettings binding
│   │       ├── Middleware/         # ASP.NET middleware
│   │       ├── Extensions/         # DI / service collection helpers
│   │       ├── Migrations/         # MongoDBMigrations versioned migrations
│   │       ├── Jobs/               # Quartz scheduled jobs
│   │       ├── WF/                 # WorkflowCore workflows
│   │       ├── BuiltIn/            # Built-in templates (mail, etc.)
│   │       ├── Utils/              # Shared utility helpers
│   │       ├── Program.cs          # Composition root
│   │       └── MappingProfile.cs   # AutoMapper profile
│   └── frontend/
│       └── mbt-spa/                # React 18 + Vite SPA
│           └── src/
│               ├── pages/          # Route-level views
│               ├── layouts/        # Page shells
│               ├── components/     # Shared UI (Radix + Tailwind, shadcn-style)
│               ├── controls/       # Form/UI primitives
│               ├── api/            # axios-based API client modules
│               ├── hooks/          # Reusable React hooks
│               ├── router/         # react-router-dom config
│               ├── dto/            # DTOs mirroring backend contracts
│               ├── types/          # Shared TS types
│               └── e2e/            # End-to-end tests
├── docs/
├── .github/
└── CLAUDE.md, STYLE.md, CONTRIBUTING.md, MANIFESTO.md, SECURITY.md, README.md, mcp.json
```

## Key Conventions

- **Backend layering:** Endpoints stay thin and dispatch to MediatR requests in `UseCases/`. Business logic lives in `Services/`. All MongoDB access goes through `Repositories/` — never call `IMongoCollection<T>` directly from endpoints, handlers, or services.
- **DTOs vs. entities:** never return MongoDB entities from endpoints. Map to DTOs via the AutoMapper `MappingProfile`.
- **Validation:** input validation uses `FluentValidation` validators placed next to the request/DTO they validate; auto-registered via DI. Do not duplicate validation in handlers or hand-roll `400`s.
- **Routing constants:** route templates live in `Endpoints/Routes.cs`, not inline.
- **Async by default:** all I/O methods are `async`, return `Task`/`Task<T>`, and accept a `CancellationToken` wired through from the endpoint.
- **Logging:** backend uses `ILogger<T>` with structured message templates (`logger.LogInformation("Loaded budget {BudgetId}", id)`) — never string interpolation. Include identifiers as structured properties.
- **Frontend components:** function components only; co-locate component + styles + tests. Use the shadcn-style wrappers in `components/ui/` rather than rebuilding Radix primitives.
- **Forms:** `react-hook-form` + `zod` resolvers. Define the zod schema next to the form and infer the type via `z.infer<typeof schema>`.
- **Data fetching:** all server state goes through `@tanstack/react-query`. API call functions live in `src/api/` and return typed promises — components do not call `axios` directly.
- **Auth:** access tokens and auth state go through `AuthContextProvider` / `useAuth`. Do not call MSAL APIs directly from feature code.
- **Tables:** use `@tanstack/react-table` for non-trivial tabular UI; follow existing column-definition patterns.

## What Not to Do

- Do not use `any` in TypeScript — prefer `unknown` + narrowing when the type is uncertain.
- Do not return MongoDB entities from endpoints; always map to DTOs.
- Do not call `IMongoCollection<T>` outside the `Repositories/` layer.
- Do not call `axios` from React components — go through `src/api/`.
- Do not call MSAL APIs directly from feature code — go through `AuthContextProvider` / `useAuth`.
- Do not invent ad-hoc error envelopes on the backend — let `ProblemDetails` produce RFC 7807 responses.
- Do not use string interpolation in log messages — use structured `ILogger<T>` templates.
- Do not commit secrets, tokens, or credentials. Use environment variables / appsettings overrides / `.env` files excluded from VCS.
- Do not log tokens, full PII request bodies, or authorization headers.
- Do not edit a merged MongoDB migration — add a new one in `src/backend/mbt.webapi/Migrations/`.
- Do not touch shared/conflict-prone files (`Program.cs`, `MappingProfile.cs`, `src/router/`, `queryClient.tsx`, `tailwind.config.js`) from more than one story per wave.
- Do not start implementation without a corresponding GitHub Issue. Every work request must be tracked.
- Do not discard the originating human prompt — capture it in the issue's "Originating Prompt" section or as a comment for retrospective use.
- Do not close, implement, or modify issues labeled `sample`. These are onboarding references (prefixed `[SAMPLE 1]`, `[SAMPLE 2]`), not actionable work items.
