# Agent Memory Rules

این فایل مکمل `AGENTS.md` است و رفتار عامل‌ها را برای Episodic Memory مشخص می‌کند.

## چه زمانی episode ثبت شود

- وقتی تصمیم معماری گرفته یا اصلاح می‌شود.
- وقتی باگ مهم کشف می‌شود.
- وقتی باگ یا incident رفع می‌شود.
- وقتی migration جدید اضافه یا اعمال می‌شود.
- وقتی تلاش ناموفق ارزش تکرار-نشدن دارد.
- وقتی dependency مهم ارتقا می‌یابد.
- وقتی deployment یا رویداد عملیاتی مهم رخ می‌دهد.
- وقتی یافته امنیتی یا کارایی مهم به دست می‌آید.

## ثبت خودکار فعلی

- backend به‌صورت خودکار unhandled exceptionهای API را به‌عنوان episode از نوع `Incident` ثبت می‌کند.
- backend به‌صورت خودکار اجرای migration/bootstrap در startup را به‌عنوان episode از نوع `Migration` یا `DeploymentEvent` ثبت می‌کند.
- این اتوماسیون fail-open است؛ یعنی اگر ثبت episode شکست بخورد، پاسخ اصلی API را خراب نمی‌کند.
- backend به‌صورت read-only سه endpoint کمکی برای agent دارد تا بدون دسترسی مستقیم به PostgreSQL وضعیت toolها، دیتابیس و memory را بررسی کند.

## چه زمانی episode جستجو شود

- قبل از تغییر schema یا migration.
- قبل از تغییر auth/security.
- قبل از بازطراحی یک ماژول ناپایدار.
- قبل از تکرار یک راه‌حل یا workaround قبلی.
- قبل از عملیات production/deployment.

## قوانین کیفیت

- episode باید خلاصه، قابل جستجو، و دارای reference پایدار باشد.
- reference پایدار یعنی file path، type name، table name، migration id، route یا commit sha.
- از ذخیره idهای ناپایدار Graphify به‌عنوان کلید اصلی جلوگیری شود.
- از ثبت episodeهای کم‌ارزش و noisy خودداری شود.

## ابزارهای مناسب برای agent

- `Episodes/SearchAsync` برای جستجوی حافظه
- `Episodes/RecentAsync` برای بررسی رخدادهای اخیر
- `AgentTools/DatabaseStatusAsync` برای بررسی connectivity و migration status
- `AgentTools/MemoryStatusAsync` برای بررسی پوشش و تازگی episodic memory

## دسترسی مستقیم به دیتابیس

- در وضعیت فعلی پروژه، agent نباید PostgreSQL را مستقیما query کند مگر در سناریوی کاملا استثنایی و کنترل‌شده.
- اولویت با endpointها و serviceهای application-level است تا boundaryهای معماری و portability حفظ شوند.
