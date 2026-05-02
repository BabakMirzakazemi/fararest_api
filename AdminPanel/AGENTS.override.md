# AdminPanel/AGENTS.override.md

Scope: This file applies to work inside `AdminPanel/**` and overrides root guidance where needed.

## Local Goals
- Keep Razor Pages handlers thin.
- Put metadata/query and reflection-heavy logic in `AdminPanel/Infrastructure/**`.
- Do not duplicate domain/business rules from `Services`; consume API/contracts when possible.

## Editing Rules
1. Prefer adding new operations in `Infrastructure/OperationCatalogService.cs` and entity metadata in `Infrastructure/EntityAdminService.cs`.
2. Keep page models (`Pages/**/*.cshtml.cs`) focused on orchestration and UI validation only.
3. Avoid writing direct data-access logic in page models.

## Verification
- Run: `dotnet build AdminPanel/AdminPanel.csproj`
- If infrastructure contracts changed across projects, run: `dotnet build babak_base.slnx`

## Output Notes
When finishing AdminPanel tasks, include:
1. Which page(s) changed
2. Which infrastructure service(s) changed
3. What command was used for verification
