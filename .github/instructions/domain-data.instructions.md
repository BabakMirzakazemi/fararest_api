---
applyTo: "Data/**/*.cs,Entities/**/*.cs"
description: "Domain and persistence boundaries"
---
# Domain and Data Rules
- `Entities` is domain-first and must not depend on API or web infrastructure.
- `Data` is persistence/infrastructure; keep EF Core details here.
- Configure entity mappings with explicit configurations under `Entities/**/Configurations` and wire in data layer.
- When entity shape changes, add/update EF Core migration under `Data/Migrations`.
- Keep repository behavior aligned with generic repository abstractions used by services.
- Do not leak persistence models directly to API responses; use DTO mapping through Services.

