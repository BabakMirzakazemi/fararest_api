# Backend Integrations - Fararest Menu Module

## ERP Integration Principle

Menu Management is the source of truth. Other modules must consume menu data rather than duplicate it.

## Required Integrations

### POS / Cashier

Consumes:

- item
- category
- price
- modifier/options
- tax/service rules
- availability status
- channel-specific rules

Must support:

- fast item lookup
- active/inactive filtering
- unavailable filtering
- channel-specific pricing
- item-level customization

### Inventory

Consumes/Provides:

- stock level
- ingredient availability
- recipe consumption
- low-stock alerts
- automatic unavailability

Rule:

If required inventory or ingredients are insufficient, item can become unavailable automatically.

### Accounting / Tax

Consumes:

- price
- tax policy
- service charge
- item financial category
- official product/service identifiers if required

### CRM / Loyalty

Consumes:

- customer profile
- past orders
- loyalty level
- coupons
- personalized offers

Provides:

- targeted campaigns
- customer-specific discounts
- profile-aware menu experience

### Online Order / Delivery

Consumes:

- active menu
- channel-specific items
- delivery/takeaway availability
- preparation time
- pricing and discount rules

### Reservation / Queue

Provides:

- table reservation
- queue state
- scheduled order time
- capacity-aware experience

### Digital Menu & Kiosk

Consumes:

- published menu version
- item images/descriptions
- prices
- stock/availability
- modifiers
- schedule rules
- branding configuration
- QR/link/campaign metadata

## Integration Risks

- Without POS integration, digital menu becomes only a showcase.
- Without inventory integration, customers see unavailable items.
- Without accounting integration, tax and invoice values can become inaccurate.
- Without CRM integration, personalization and loyalty are weak.
- Without versioning and audit, large teams cannot safely manage changes.
