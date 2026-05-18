# Frontend UX Principles - Digital Menu, Kiosk & Menu Management

## Global UX Principles

- فارسی‌محور و راست‌به‌چپ
- ساده، سریع و قابل فهم بدون آموزش
- موبایل‌فرست برای منوی دیجیتال
- لمس‌فرست برای کیوسک
- مناسب کاربر ایرانی و سناریوهای واقعی شعبه
- کاهش بار ذهنی با مرحله‌بندی فرم‌ها
- نمایش شفاف قیمت، موجودی، زمان آماده‌سازی و هزینه‌های جانبی
- پیش‌نمایش قبل از انتشار

## Menu Management UX

### Dashboard

Should show:

- active items count
- inactive items count
- unavailable items count
- categories count
- pending approval items
- best-selling items
- warnings:
  - item without image
  - item without category
  - item without price
  - item without preparation time
  - item with incomplete recipe

### Menu List

Should support:

- card/table switch
- instant search
- filters by name, code, category, branch, status, price, item type, channel
- inline edit for price, active status, availability, sort order and tags
- multi-select actions

### Add/Edit Item Form

Should be step-based:

1. Basic information
2. Price and sales rules
3. Inventory/preparation
4. Image and descriptions
5. Ingredients/recipe
6. Channels and branches

Important rule:

Fields must change based on item type. A packaged product should not see unnecessary produced-food recipe fields.

## Digital Menu UX

Customer should quickly understand:

- view menu
- order/reserve
- contact/get guidance

Must include:

- clear categories
- fast search
- item cards with image, description, price and availability
- item tags
- customization flow
- transparent cart
- payment/receipt flow
- recovery of abandoned cart if possible

## Kiosk UX

Must include:

- large touch targets
- short path to order
- queue-friendly flow
- auto reset after inactivity
- clear confirmation
- visual-first interaction
- support for self-service payment if enabled

## Digital Signage UX

Can be non-interactive.

Should support:

- rotating menus
- bundles
- campaigns
- prices
- high readability from distance
- time-based content
