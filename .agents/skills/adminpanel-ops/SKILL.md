---
name: adminpanel-ops
description: Use for AdminPanel Razor Pages admin tooling tasks in this repository, including operation catalog invocation, entity admin metadata/list/edit/delete workflows, and infrastructure alignment.
---

# AdminPanel Ops Skill

Use this skill when a task touches `AdminPanel/**` pages or infrastructure.

## Scope
- Operation discovery/invocation: `AdminPanel/Infrastructure/OperationCatalogService.cs`
- Entity metadata/CRUD admin logic: `AdminPanel/Infrastructure/EntityAdminService.cs`
- Dependency registration: `AdminPanel/Infrastructure/AdminPanelServiceCollectionExtensions.cs`
- UI orchestration pages: `AdminPanel/Pages/Operations/**`, `AdminPanel/Pages/Entities/**`

## Workflow
1. Read nearest instruction files first:
   - `AGENTS.md`
   - `AdminPanel/AGENTS.override.md`
2. Decide if change belongs to:
   - infrastructure service logic (preferred)
   - Razor PageModel orchestration (thin)
   - page template/view concerns
3. Keep reflection/query/metadata heavy logic in `Infrastructure`.
4. Keep PageModel focused on request binding, calling service, and rendering results.

## Operation Catalog Checklist
- New operation should come from registered service interfaces under `Services.Contracts`.
- Ensure method parameter conversion behavior remains explicit and safe.
- Keep cancellation token handling intact.
- Return clear success/error messages from invocation layer.
- Keep reflection paths null-safe (`GetMethod`, metadata reads, dynamic invocation inputs/outputs).
- Avoid unsafe direct casts; prefer `as`/pattern matching + validation error for incompatible payloads.

## Entity Admin Checklist
- Keep key-token parsing/building consistent.
- Preserve validation and conversion flow before save.
- Keep pagination/search/default ordering behavior deterministic.
- For FK fields, maintain lookup option generation and labels.
- For heavy list/search queries, annotate with `TagWith("Feature.Endpoint")` where query is composed.
- Use `AsNoTracking` for read-focused grid/list screens unless update tracking is explicitly needed.
- If query/index strategy is pending real production workload, add explicit `TODO(index)`/`TODO(query-tuning)` notes near implementation points.
- Keep telemetry correlation fields available in operation logs so admin actions can be traced across layers.

## DI and Safety
- If a new service is needed by AdminPanel tools, register it in `AdminPanelServiceCollectionExtensions`.
- Avoid introducing direct business logic in pages.
- Avoid duplicating domain rules that already exist in `Services`.
- Keep nullability contracts explicit in infrastructure helpers and page models.
- Do not silence compiler warnings with broad suppressions; fix root cause.

## Verification Commands
- `dotnet build AdminPanel/AdminPanel.csproj`
- If shared contracts changed: `dotnet build babak_base.slnx`
- `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1`

If build cannot run due environment/network restrictions, report it explicitly.
