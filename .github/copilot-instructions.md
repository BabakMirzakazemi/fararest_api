# Copilot Instructions

Read and follow repository root `AGENTS.md` as the primary instruction source.
Persian mirror is available at `AGENTS.fa.md`.

## Mandatory rules
- Respect Clean/Onion boundaries exactly as defined in `AGENTS.md`.
- Keep controllers thin; move business logic to `Services`.
- Keep domain concerns in `Entities`.
- Keep EF/persistence in `Data`.
- Keep web pipeline/DI/middleware in `WebFramwork`.
- Use existing exception and validation patterns.
- Apply minimal, localized edits and avoid unrelated refactors.

## Before finalizing changes
- Build successfully.
- Run tests if available.
- Summarize changed files and architectural reasoning.

