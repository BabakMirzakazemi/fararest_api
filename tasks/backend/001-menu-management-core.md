# Task: Implement Menu Management Core

## Type

Backend

## Required Context

- `knowledge/menu-and-digital-menu/business/module-overview.md`
- `knowledge/menu-and-digital-menu/business/business-rules.md`
- `knowledge/menu-and-digital-menu/business/glossary.md`
- `knowledge/menu-and-digital-menu/backend/domain-model.md`
- `knowledge/menu-and-digital-menu/backend/api-boundaries.md`

## Goal

Implement the core backend structure for menu, category and item management.

## Scope

Implement only:

- menu entity/model
- category entity/model
- item entity/model
- item type
- active/inactive status
- basic CRUD
- soft delete/archive behavior
- unique item code validation

## Out of Scope

- digital menu public UI
- kiosk
- cart/order
- payment
- advanced analytics
- AI recommendation
- inventory integration
- CRM integration

## Acceptance Criteria

- A menu can be created and updated.
- A category can be created under a menu.
- A category can have parent/child hierarchy.
- An item can be created under a category.
- Item must have name, type, category, status and price.
- Duplicate unique item code is rejected.
- Delete action archives instead of hard deleting.
