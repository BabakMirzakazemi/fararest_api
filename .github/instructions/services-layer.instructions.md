---
applyTo: "Services/**/*.cs"
description: "Application/service layer rules"
---
# Services Layer Rules
- Put business and use-case logic in this layer.
- Keep service contracts in `Services/Contracts/**` and implementations in `Services/Services/**`.
- Validate input close to DTOs/services with existing FluentValidation patterns.
- Depend on abstractions (`IRepository`, `IServiceRepository`) instead of infrastructure details.
- Throw domain/application exceptions using existing `Common.Exceptions` types.
- Avoid references to API/presentation concerns.
- Keep methods focused and testable.

