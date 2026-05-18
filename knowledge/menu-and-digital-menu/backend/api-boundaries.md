# Backend API Boundaries - Fararest Menu Module

## Backend Responsibilities

Backend should own:

- menu structure
- category hierarchy
- item master data
- item type behavior
- pricing rules
- tax/service rules
- availability rules
- branch/channel/time scoping
- recipe and inventory links
- modifier groups and options
- publishing/versioning
- audit logging
- access control
- integration events

## Suggested API Groups

### Menu Management APIs

- create/update/archive menu
- create/update/archive category
- reorder categories/items
- create/update/archive item
- change item status
- bulk update prices/statuses
- clone menu/category/item
- import/export menu data

### Pricing APIs

- set base price
- set branch price
- set channel price
- set scheduled/campaign price
- validate abnormal price changes

### Availability APIs

- manual unavailable/available
- schedule availability
- get resolved availability
- inventory-driven availability update

### Modifier APIs

- create/update modifier groups
- create/update modifier options
- validate required/optional selections
- calculate modifier price impact

### Publishing APIs

- create draft version
- preview version
- schedule publish
- publish now
- rollback version

### Digital Menu APIs

- get public menu by link/QR
- get kiosk menu by device/channel
- get signage playlist/menu
- create cart
- update cart
- submit order
- generate receipt/pre-invoice

### Analytics APIs

- track scan/view/click
- track item view
- track cart event
- track checkout drop-off
- report menu performance

## Do Not Mix

Menu Management APIs should not directly own UI layout rendering.

Digital Menu APIs should not mutate master menu data except through controlled operational actions such as order/cart events.
