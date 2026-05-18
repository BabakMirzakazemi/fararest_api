# Backend Domain Model - Fararest Menu Module

## Aggregate Areas

### Menu

Represents a sellable menu structure.

Suggested fields:

- id
- name
- businessType
- branchId / brandId / conceptId
- status
- activeChannels
- visibilityRules
- version
- publishedAt
- createdAt
- updatedAt

### Category

Represents menu grouping.

Suggested fields:

- id
- parentId
- menuId
- name
- internalDescription
- displayDescription
- image
- color
- sortOrder
- isActive
- visibleChannels
- branchScope

### Item

Represents sellable entity.

Suggested fields:

- id
- name
- uniqueCode
- itemType
- categoryId
- shortDescription
- longDescription
- image
- tags
- basePrice
- taxPolicy
- servicePolicy
- preparationTime
- status
- availabilityStatus
- branchScope
- channelScope
- scheduleRules
- createdAt
- updatedAt

### PriceRule

Supports pricing variation.

Suggested fields:

- id
- itemId
- type: base | branch | channel | time | campaign
- branchId
- channel
- startAt
- endAt
- amount
- minAmount
- maxAmount
- requiresApproval

### AvailabilityRule

Controls sellability.

Suggested fields:

- id
- itemId
- branchId
- channel
- status
- reason
- source: manual | inventory | schedule
- startAt
- endAt

### Recipe

For produced items.

Suggested fields:

- id
- itemId
- version
- components
- costCalculationMode
- isActive

### ModifierGroup

For customization.

Suggested fields:

- id
- itemId
- name
- isRequired
- minSelect
- maxSelect
- sortOrder

### ModifierOption

Suggested fields:

- id
- modifierGroupId
- name
- priceDelta
- ingredientImpact
- isActive

### MenuVersion

For publishing and rollback.

Suggested fields:

- id
- menuId
- versionNumber
- status: draft | scheduled | published | archived
- publishAt
- createdBy
- approvedBy
- changelog

### AuditLog

Tracks sensitive changes.

Suggested fields:

- id
- entityType
- entityId
- action
- before
- after
- actorId
- createdAt

## Important Backend Constraints

- Prefer soft delete/archive for business entities.
- Price and availability must be resolved by branch, channel and time.
- Item type determines required/visible fields.
- Published menu should use stable versioned snapshots.
- Integration-facing APIs must avoid inconsistent item, price or availability data.
