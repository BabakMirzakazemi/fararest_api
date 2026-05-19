#  Business Spec

## Scope
- Menu, category and item management as the source of truth for sellable menu data.
- Core CRUD for menu/category/item with archive instead of hard delete.
- Category parent/child hierarchy inside one menu.
- Item master data including unique code, item type, price and active/inactive status.

## Actors and Permissions
- TODO

## Core Use Cases
- TODO

## Business Rules
- Every menu must belong to a real registered organization/business.
- Category and item records inherit their organization scope from the validated menu/category chain and must not be created under an orphaned organization reference.

## Validation Rules
- `OrganizationId` for menu creation must reference an existing organization/business record.
- If the referenced organization/business does not exist, the API must reject the request with a clear validation error before creating menu/category/item data.

## Data Contracts
- TODO

## Events and Side Effects
- TODO

## Non-Functional Requirements
- TODO

## Open Questions
- Should archived item codes stay permanently reserved, or can a future restore/reuse policy release them?
- Should category names be unique per menu, or are duplicates under different hierarchy branches acceptable?
- When branch/channel pricing is added later, should the current `PriceAmount` remain the single base price source of truth?
