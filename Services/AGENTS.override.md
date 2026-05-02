# Services/AGENTS.override.md

Scope: This file applies to `Services/**` and refines root architecture guidance.

## Local Rules
- `Services/Contracts/**` remains interface-only.
- `Services/DTOs/**` holds transport models and validators.
- `Services/Services/**` contains application use-case logic.
- Keep external delivery concerns (HTTP/controller specifics) out of this layer.

## Authentication Change Checklist
When changing auth/account flow:
1. Update DTO request/response types.
2. Add/update FluentValidation validators.
3. Update service contract in `Contracts/Authentications`.
4. Update implementation in `Services/Authentications`.
5. Ensure API layer endpoints are aligned.

## Safety Rules
- Never bypass validation assumptions in service logic; enforce critical checks again server-side.
- Keep OTP expiry and account activation checks explicit and centralized.

## Verification
- Run: `dotnet build Services/Services.csproj`
- If endpoint contract changed, also run: `dotnet build API/API.csproj`
