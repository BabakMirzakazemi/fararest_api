---
applyTo: "Services/**/*.cs"
description: "قواعد فارسی لایه سرویس"
---
# قواعد Services
- منطق use-case و کسب‌وکار در این لایه باشد.
- قراردادها در `Services/Contracts` و پیاده‌سازی‌ها در `Services/Services`.
- Validation نزدیک DTO/Service و مطابق الگوی فعلی باشد.
- وابستگی‌ها به abstractionها باشد.
- وابستگی به API ایجاد نشود.
