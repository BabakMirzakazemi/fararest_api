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

## 2026-05-17 - Prefer MCP-safe application adapters over direct PostgreSQL access

- Decision: برای بهبود agent، یک سطح read-only و application-safe از tool endpointها اضافه شد و دسترسی مستقیم MCP به PostgreSQL فعلا کنار گذاشته شد.
- Reason:
  - boundaryهای Onion/Clean بهتر حفظ می‌شوند.
  - agent به status و memory insight موردنیازش می‌رسد بدون این‌که SQL آزاد اجرا کند.
  - portability بین PostgreSQL و SQL Server کمتر آسیب می‌بیند.
- Consequence:
  - endpointهای `AgentTools` به‌عنوان سطح مناسب‌تر برای adapterهای MCP معرفی شدند.
  - اگر در آینده direct DB tooling لازم شود، باید محدود، read-only و کاملا audit-friendly باشد.
