# AGENTS.md

## Language Versions
- English primary: `AGENTS.md`
- Persian mirror: `AGENTS.fa.md`

## Hierarchical Instruction Files
- Use the closest instruction file to the code being changed.
- Root defaults live in `AGENTS.md`.
- Local overrides may exist as `AGENTS.override.md` in subdirectories.
- For this repository:
  - `AdminPanel/AGENTS.override.md`
  - `Services/AGENTS.override.md`

## Repo Skills
- Repository-specific reusable skills may exist under `.agents/skills/**`.
- Prefer those skills for repetitive workflows before re-deriving process each task.
- Current repo skills:
  - `.agents/skills/dotnet-backend-auth/SKILL.md`
  - `.agents/skills/adminpanel-ops/SKILL.md`
  - `.agents/skills/auth-business/SKILL.md`
  - `.agents/skills/accounts-business/SKILL.md`
  - `.agents/skills/licenses-business/SKILL.md`
  - `.agents/skills/payments-business/SKILL.md`
  - `.agents/skills/menu-business/SKILL.md`
  - `.agents/skills/digital-menu-business/SKILL.md`
  - `.agents/skills/support-business/SKILL.md`
  - `.agents/skills/crm-business/SKILL.md`

## Business Requirements Source
- Business requirement documents live under `docs/business/**`.
- Use `docs/business/README.md` as the entry point.
- For any implementation in domains `auth/accounts/licenses/payments/menu/digital-menu/support/crm`:
  1. Read the related domain document first.
  2. Follow the related domain skill under `.agents/skills/*-business/SKILL.md`.
  3. If requirements are ambiguous, add an explicit `Open Questions` item back to the related document.

## Purpose
This file defines mandatory engineering and architecture rules for AI agents working in this repository.
All agents must read and follow this file before making any change.

## Project Snapshot
- Solution style: ASP.NET Core Web API
- Architecture: Clean/Onion
- Main layers in this repo:
  - `Entities` (Domain)
  - `Services` (Application + Contracts + DTOs)
  - `Data` (Infrastructure, EF Core)
  - `API` (Presentation)
  - `WebFramwork` (Cross-cutting web infrastructure)
  - `Common` (Shared kernel)

## Non-Negotiable Layer Dependency Rules
Follow this direction strictly:
1. `Entities` must not depend on `Data`, `API`, or `WebFramwork`.
2. `Services` can depend on `Entities` and `Common`, but not on `API`.
3. `Data` can depend on `Entities` and `Common`.
4. `API` should be thin; it may depend on `Services` contracts/DTOs and `WebFramwork`.
5. `WebFramwork` is infrastructure glue; do not move business logic here.
6. `Common` must stay generic and reusable; no feature-specific business logic.

If a task requires violating these rules, stop and propose a refactor path instead of direct violation.

## File Ownership By Concern
- Domain entities and enums: `Entities/**`
- EF Core context and migrations: `Data/**`
- Business services and interfaces: `Services/**`
- API endpoints/controllers: `API/Controllers/**`
- DI, middleware, authentication wiring: `WebFramwork/**`
- Shared utilities and base exceptions: `Common/**`

## Implementation Rules
1. Keep controllers thin.
   - Controllers should orchestrate request/response only.
   - Put business rules in `Services`.
2. Register dependencies through existing DI conventions.
   - Use marker interfaces (`IScopedDependency`, `ITransientDependency`, `ISingletonDependency`) where appropriate.
   - Respect Autofac registration patterns in `WebFramwork/Configuration/IoCConfigurations/AutofacModule.cs`.
3. Validation must live near request DTOs/services, not inside controllers.
4. Use existing exception model (`AppException`, etc.) for business errors.
5. Keep mapping centralized through existing AutoMapper setup.
6. Never hardcode secrets or environment-specific values in source code.
7. Keep nullability contracts honest:
   - If a value can be absent, declare it nullable (`?`).
   - Do not return `null` from non-nullable return types.
8. For EF/domain model safety:
   - Initialize non-null properties via constructor/default/`required`.
   - Use `null!` only when a framework/ORM guarantees population (for example EF navigation), and keep usage minimal.
9. For reflection/framework metadata safety:
   - Guard nullable framework values (`GetEntryAssembly()`, `GetRuntimeMethods()`, `EndpointMetadata`, swagger operation collections).
   - Avoid blind casts from `object`; use safe cast + null guards/fallbacks.
10. JWT/auth reliability rules:
   - Keep token expiry units explicit and consistent with settings (minutes means `AddMinutes`).
   - In token validation events, fail authentication only under explicit failed checks (no unconditional `Fail`).
11. Query tuning baseline rules:
   - Add `TagWith("Feature.Endpoint")` for heavy/read-hot EF queries to make plan tracing observable.
   - For new high-traffic list/search endpoints, define expected sort/filter pattern and indexing TODO in code.
   - Do not add speculative indexes blindly; validate with execution plans and real workload metrics.
12. Telemetry baseline rules:
   - Keep request telemetry middleware enabled in non-test environments unless there is a deliberate operational reason.
   - Preserve structured trace context in logs (`TraceId`/`SpanId`) to correlate logs with slow-query and request metrics.
   - Add business/KPI telemetry tags only after domain semantics are stable; avoid noisy high-cardinality tags.

## Warning & Quality Gate Policy
1. Repository target is warning-free (`0` compiler warnings).
2. New warnings are treated as regressions and must be fixed before finalizing a task.
3. Do not hide warnings with broad `#pragma` or global suppression unless explicitly approved with rationale.
4. Run warning gate before completion:
   - `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1`
5. Update warning baseline only for intentional, reviewed cleanup:
   - `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1 -UpdateBaseline`

## Task Execution Protocol (Agent Workflow)
For every task:
1. Read impacted layer(s) first.
2. Identify architecture boundary impact.
3. Apply minimal, localized changes.
4. Update/add DTOs, validators, services, and controller endpoints in correct layers.
5. Run build/tests (if available) before finalizing.
6. Summarize changed files and architecture reasoning.

## Done Criteria (Definition of Done)
A task is complete only if:
1. Code compiles.
2. No architecture boundary violation is introduced.
3. New behavior is wired through DI and reachable from API.
4. Any required DB migration is included (when model changed).
5. Relevant tests are added/updated if test project exists.
6. No new compiler warning is introduced relative to the baseline.
7. For data-heavy changes, query observability remains intact (slow-query logging + query tags where applicable).

## Practical Examples

### Example A: Add a new use-case (feature)
- Create request/response DTOs in `Services/DTOs/...`.
- Add/extend service contract in `Services/Contracts/...`.
- Implement logic in `Services/Services/...`.
- If persistence is needed, use repository abstractions and data-layer support.
- Expose endpoint in `API/Controllers/...`.

### Example B: Add a new entity field persisted in DB
- Update entity/configuration in `Entities/**`.
- Update EF mapping/context in `Data/**` if needed.
- Add migration in `Data/Migrations/**`.
- Update DTOs/services/API contract accordingly.

### Example C: Authentication/Authorization changes
- Keep token/cookie/auth pipeline updates in `WebFramwork/Configuration/**`.
- Keep user/business verification logic in `Services`.
- Do not place auth business rules directly in controllers.

## Change Safety Rules
1. Do not rename folders/projects unless task explicitly requires it.
2. Do not introduce new frameworks/packages without clear need.
3. Do not modify migration history destructively.
4. Keep public API contracts backward compatible unless task explicitly allows breaking change.

## Suggested Local Verification Commands
Run from repository root:
- `dotnet restore`
- `dotnet build`
- `dotnet test` (if test projects exist)
- `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1`

## Agent Output Format (for PRs/Task reports)
Always provide:
1. What changed
2. Why this layer placement is correct
3. How it was verified
4. Any risks/follow-ups

## Mandatory HTML Documentation Versioning Rules
These rules are mandatory for every future change and are part of Definition of Done for related changes.

1. Database entity change documentation:
   - Trigger condition: any change in `Entities/**` and/or EF entity configuration/migration that changes database shape or semantics.
   - Output folder: `database-documents/`
   - File naming pattern: `database-document-v{N}.html`
   - Versioning rule: never overwrite previous versions; create a new file with next sequential version (`v1`, `v2`, `v3`, ...).
   - Content rule: follow the same content style as `database-documents/fararest-api-authz-entities-v2.html`.
   - Scope rule: document the full database entities model (not only the changed entity), while clearly highlighting what changed in this version.

2. Program flow change documentation:
   - Trigger condition: any behavior change in auth, authorization, business flow, endpoint behavior, service orchestration, or business rules.
   - Output folder: `flow-documents/`
   - File naming pattern: `flow-documents-v{N}.html`
   - Versioning rule: never overwrite previous versions; create a new file with next sequential version (`v1`, `v2`, `v3`, ...).
   - Content rule: include latest end-to-end flow behavior and explicitly list impacted APIs and tables per section.

3. Operational rule:
   - The latest version number (`N`) must be discovered from existing files in the target folder and incremented by 1.
   - If no prior versioned file exists, start from `v1`.
   - Creating these files is mandatory whenever the trigger conditions above are met.

# Graphify Project Context

This repository is a .NET project.

Graphify has already been initialized for this repository and should be used as the primary project understanding and token-saving layer.

## Graphify Outputs

The Graphify output directory is:

- `graphify-out/graph.json`
- `graphify-out/GRAPH_REPORT.md`
- `graphify-out/graph.html`
- `graphify-out/manifest.json`

## Graphify Refresh Command

When Graphify data is missing, outdated, or stale, refresh it from the repository root with:

```bash
python -m graphify update .
```

Important environment note:
- In this repository environment, direct `graphify` command may not be available on PATH.
- Always use: `python -m graphify ...`

## Graphify-First Rules

1. Before opening raw source files, inspect Graphify outputs first.
2. Use graph data to identify:
   - relevant files
   - symbols
   - dependencies
   - upstream/downstream impact
3. Open only the minimum required code snippets after graph-based narrowing.
4. If Graphify answers the question sufficiently, do not read additional source files.

## Token Optimization Rules

1. Prefer:
   - `graphify-out/GRAPH_REPORT.md`
   - `graphify-out/graph.json`
   - symbol/edge-level graph queries
   - focused snippets
2. Avoid:
   - full repository scans
   - full file dumps
   - re-reading unchanged files
   - broad recursive searches unless graph data is insufficient
3. Keep context compact and task-focused.

## .NET Project Understanding Rules (via Graphify)

For architecture understanding, use Graphify to map:
- Controllers
- Services
- Repositories
- Interfaces and DI bindings
- Middleware
- EF Core DbContext and model relationships
- API route-to-service relationships
- Shared libraries and cross-layer dependencies
- Test coverage relationships (if test projects exist)

## Graph Freshness Rules

1. Refresh graph after meaningful code changes affecting structure, symbols, or dependencies.
2. Re-run:
   - `python -m graphify update .`
3. Treat graph as stale when commit changes significantly or graph output timestamp is old.

## Required Response Format For Future Coding Tasks

For each task, report in this order:
1. Graph Context
   - relevant modules
   - symbols
   - dependencies
   - impacted layers
2. Plan
   - concise minimal steps
3. Changes
   - affected files only
   - short explanation
4. Impact Analysis
   - upstream/downstream effects
   - related tests to run
5. Token Optimization Note
   - what files/context were intentionally not loaded

---
If this file conflicts with direct maintainer instructions in a task, maintainer instructions win for that task.

