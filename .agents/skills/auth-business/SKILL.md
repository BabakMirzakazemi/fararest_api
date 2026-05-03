# AUTH Business Skill

## Purpose
Use this skill when implementing authentication, account recovery, session security, and MFA flows.

## Source of Truth
- Primary doc: `docs/business/auth.md`
- Index: `docs/business/README.md`
- Input references:
  - `FaraRest_authentication_best_practices.docx`
  - `FaraRest_authentication_best_practices.pdf`

## Implementation Checklist
1. Read `docs/business/auth.md` and extract:
   - core use-cases
   - business/validation rules
   - phased delivery boundaries
2. Implement by layer:
   - `Services/DTOs/**`: request/response contracts per flow.
   - `Services/Contracts/**`: explicit auth interfaces.
   - `Services/Services/**`: business rules, OTP policy, session invalidation, MFA enforcement.
   - `Data/Entities/**` + `Data/Migrations/**`: only required schema changes.
   - `API/Controllers/**`: thin endpoints, no domain logic.
3. Enforce mandatory behaviors:
   - neutral responses for forgot password and account existence.
   - password never stored raw.
   - OTP expiration/retry/rate-limit.
   - revoke sessions on password change.
   - MFA mandatory for sensitive roles.
4. Keep architecture boundaries from `AGENTS.md`.
5. Validate with:
   - `dotnet build`
   - warning baseline script
   - runtime smoke for login/register/recovery/session flows.

## Output Expectations
- List changed files
- Explain layer placement
- State verification steps
- Mention risks/open questions

## Guardrails
- Do not leak account existence in public responses.
- Do not add auth logic in controllers or middleware glue if it belongs to services.
- Keep token lifetime units explicit and consistent with config.
- Prefer UTC timestamps for security events and expirations.
