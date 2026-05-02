# راهنمای Copilot (فارسی)

قوانین `AGENTS.md` و `AGENTS.fa.md` را به‌عنوان مرجع اصلی رعایت کن.

## قواعد اجباری
- مرزهای Clean/Onion حفظ شود.
- Controllerها thin بمانند.
- منطق کسب‌وکار در `Services` باشد.
- concerns دامنه در `Entities` بماند.
- EF/persistence در `Data` بماند.
- DI و middleware در `WebFramwork` مدیریت شود.
- تغییرات حداقلی و موضعی باشند.

## قبل از نهایی‌سازی
- Build موفق.
- اجرای تست‌ها در صورت وجود.
- خلاصه فایل‌های تغییرکرده و دلیل معماری.
