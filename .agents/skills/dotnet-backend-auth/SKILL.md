---
name: dotnet-backend-auth
description: Use for authentication/account workflow tasks in this repository (email/phone OTP registration, login flow updates, auth DTO/validator/service/controller alignment, and .NET verification commands).
---

# Dotnet Backend Auth Skill

Use this skill when the task touches authentication or account flows in this repository.

## Scope
- API auth endpoints: `API/Controllers/Auth/**`
- Contracts: `Services/Contracts/Authentications/**`
- Auth implementations: `Services/Services/Authentications/**`
- DTOs and validators: `Services/DTOs/Accounts/**`

## Workflow
1. Read the nearest `AGENTS.md` / `AGENTS.override.md` first.
2. Identify whether the change affects:
   - request DTOs
   - validators
   - service contract
   - service implementation
   - API endpoint surface
3. Apply minimal edits and keep naming consistent across layers.
4. Re-check activation/OTP/security edge cases (expired OTP, inactive user, duplicate contact).

## Required Consistency Checks
- Every new request DTO has a validator when applicable.
- Service interface and implementation signatures match.
- Controller endpoint method and DTO signatures match service contract.
- Login responses include required identity fields when expected by UI/API consumers.
- Nullability contracts are explicit (`?` for optional data), and no `null` is returned from non-nullable members.
- Avoid blind casts from `object` payloads; use safe cast + explicit fallback/guard.
- For heavy auth/account list/search queries, use `TagWith("Feature.Endpoint")` and keep query projection lean.
- Prefer `AsNoTracking` for read-only query paths to reduce EF change-tracking overhead.
- When a high-traffic query may need indexing but workload is not finalized, add a clear `TODO(index)` near the query/config.
- Preserve request telemetry correlation in auth logs (`TraceId`/`SpanId`) for incident debugging.
- Add auth-specific KPI tags/metrics only when values are low-cardinality and operationally actionable.

## Auth Hardening Rules
- Token expiry unit must match configuration semantics (minutes => `AddMinutes`).
- In token validation callbacks/events, call fail only when explicit validation checks fail.
- Keep secrets/keys out of source code (use configuration/environment/user-secrets).
- Keep controller auth endpoints orchestration-only; core auth checks remain in `Services`.

## Verification Commands
Run from repo root when possible:
- `dotnet build Services/Services.csproj`
- `dotnet build API/API.csproj`
- `dotnet build AdminPanel/AdminPanel.csproj` (if shared auth contract affects admin UI)
- `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1`

If restore/build cannot run due environment restrictions, report that explicitly.
