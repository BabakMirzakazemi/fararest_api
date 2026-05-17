# Decision Log

## 2026-05-17 - Introduce Episodic Memory inside existing Onion stack

- Decision: Episodic Memory was implemented as a standard application feature inside the current .NET solution.
- Reason:
  - کم‌ریسک‌ترین مسیر برای هماهنگی با ساختار فعلی repo بود.
  - نیازی به framework جدید مانند MediatR نداشت.
  - portability بین PostgreSQL و SQL Server بهتر حفظ شد.
- Consequence:
  - episodeها با EF Core و جداول relation-based ذخیره می‌شوند.
  - برای جستجو از schema نرمال‌شده و referenceهای پایدار استفاده می‌شود.
  - mapping این feature در `Data` نگه داشته شد تا domain از EF-specific concern جدا بماند.
