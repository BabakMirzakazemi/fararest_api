---
applyTo: "API/Controllers/**/*.cs"
description: "API controller rules for thin presentation layer"
---
# API Controller Rules
- Keep controllers thin: request mapping, authorization, and response shaping only.
- Do not implement business logic in controllers.
- Call service contracts from `Services.Contracts`.
- Use DTOs from `Services/DTOs` for request and response models.
- Return standardized API responses aligned with existing WebFramework filters.
- Do not access `DbContext` directly from controllers.
- Place authentication and authorization attributes consistently with existing controllers.

