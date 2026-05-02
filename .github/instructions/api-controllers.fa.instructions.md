---
applyTo: "API/Controllers/**/*.cs"
description: "قواعد فارسی برای کنترلرهای API"
---
# قواعد Controller
- Controller فقط درخواست/پاسخ را orchestrate کند.
- منطق کسب‌وکار داخل Controller قرار نگیرد.
- از قراردادهای `Services.Contracts` استفاده شود.
- از `DbContext` مستقیم در Controller استفاده نشود.
