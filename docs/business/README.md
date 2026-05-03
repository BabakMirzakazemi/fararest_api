# Business Docs Index

This folder is the single source of truth for business requirements used by implementation tasks.

## Domains
- [Auth](./auth.md)
- [Accounts](./accounts.md)
- [Licenses](./licenses.md)
- [Payments](./payments.md)
- [Menu](./menu.md)
- [Digital Menu](./digital-menu.md)
- [Support](./support.md)
- [CRM](./crm.md)

## How To Use
1. Write business rules and acceptance criteria in each domain file.
2. Keep API contracts, edge cases, and validation rules explicit.
3. Update related domain skill under `.agents/skills/*-business/SKILL.md` when requirements change.

## Template
Use this structure in each domain file:
- Scope
- Actors and Permissions
- Core Use Cases
- Business Rules
- Validation Rules
- Data Contracts (request/response)
- Events/Side Effects
- Non-Functional Requirements
- Open Questions
