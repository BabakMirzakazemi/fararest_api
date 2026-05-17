# Episodic Memory Skill

این skill برای عامل‌ها و توسعه‌دهندگان تعریف می‌کند که Episodic Memory چگونه استفاده شود.

## ورودی‌های مهم

- عنوان رویداد
- نوع رویداد
- اهمیت
- خلاصه
- جزئیات
- زمان وقوع
- tagها
- referenceهای ساختاری

## خروجی مورد انتظار

- یک episode قابل جستجو
- دارای `DeduplicationKey`
- دارای tagهای نرمال‌شده
- دارای referenceهای پایدار به ماژول، entity، migration، endpoint یا فایل

## عدم استفاده

- برای facts و قوانین پایدار از semantic memory استفاده شود.
- برای روابط ساختاری از graph memory و Graphify استفاده شود.
- Episodic Memory فقط برای eventها و outcomeهای زمان‌مند است.
