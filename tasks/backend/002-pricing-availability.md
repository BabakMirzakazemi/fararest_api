# Task: Implement Pricing & Availability Rules

## Type

Backend

## Required Context

- `knowledge/menu-and-digital-menu/business/business-rules.md`
- `knowledge/menu-and-digital-menu/backend/domain-model.md`
- `knowledge/menu-and-digital-menu/backend/api-boundaries.md`

## Goal

Add branch/channel/time-aware pricing and availability to menu items.

## Scope

Implement only:

- base price
- branch-specific price
- channel-specific price
- manual available/unavailable status
- temporary unavailability
- schedule-based availability
- resolved price/availability service

## Out of Scope

- payment
- accounting integration
- inventory-driven auto unavailability
- coupons
- campaign engine
- frontend UI

## Acceptance Criteria

- System can resolve item price for branch + channel + time.
- System can resolve item availability for branch + channel + time.
- Manual unavailable overrides normal availability.
- Invalid price values are rejected.
- Changes are audit-logged if audit module exists.
