# AGENTS.fa.md

## هدف
این فایل قوانین اجباری مهندسی و معماری را برای Agentهای هوش مصنوعی مشخص می‌کند.
تمام Agentها باید قبل از هر تغییر این فایل را بخوانند.

## دستورالعمل‌های سلسله‌مراتبی
- نزدیک‌ترین فایل دستورالعمل به مسیر کد، اولویت دارد.
- قواعد پایه در `AGENTS.md` قرار دارند.
- قواعد محلی می‌توانند در فایل `AGENTS.override.md` در زیرشاخه‌ها تعریف شوند.
- در این مخزن:
  - `AdminPanel/AGENTS.override.md`
  - `Services/AGENTS.override.md`

## Skillهای ریپویی
- Skillهای قابل استفاده مجدد در `.agents/skills/**` نگهداری می‌شوند.
- برای کارهای تکراری، قبل از طراحی مجدد فرآیند از Skill مربوطه استفاده شود.
- Skillهای فعلی:
  - `.agents/skills/dotnet-backend-auth/SKILL.md`
  - `.agents/skills/adminpanel-ops/SKILL.md`

## مرور اجباری کانتکست قبل از هر تسک
- قبل از رسیدگی به هر prompt یا task جدید، مسیر `knowledge/**` برای دانش پروژه، تصمیم‌ها و کانتکست عملیاتی بررسی شود.
- قبل از رسیدگی به هر prompt یا task جدید، فایل‌های راهنمای بیزینس مرتبط در `docs/business/**` از `docs/business/README.md` شروع و بررسی شوند.
- هر جا که نیازمندی‌ها، اصطلاحات، workflowها یا محدودیت‌ها ممکن است تحت تاثیر باشند، `knowledge/**` و `docs/business/**` منبع کانتکست اجباری محسوب می‌شوند.
- اگر task ظاهراً نامرتبط با behavior بیزینسی بود، باز هم ابتدا یک بررسی سریع برای سنجش ارتباط این منابع انجام شود و فقط در صورت بی‌ارتباط بودن از مرور عمیق صرف‌نظر شود.

## نمای کلی پروژه
- نوع پروژه: ASP.NET Core Web API
- معماری: Clean/Onion
- لایه‌ها:
  - `Entities` (دامنه)
  - `Services` (اپلیکیشن + قراردادها + DTOها)
  - `Data` (زیرساخت و EF Core)
  - `API` (ارائه)
  - `WebFramwork` (زیرساخت مشترک وب)
  - `Common` (اشتراکی)

## قوانین وابستگی لایه‌ها (غیرقابل مذاکره)
1. `Entities` نباید به `Data`، `API` یا `WebFramwork` وابسته شود.
2. `Services` می‌تواند به `Entities` و `Common` وابسته باشد، نه به `API`.
3. `Data` می‌تواند به `Entities` و `Common` وابسته باشد.
4. `API` باید thin بماند و فقط orchestration انجام دهد.
5. `WebFramwork` فقط برای wiring/زیرساخت است، نه منطق کسب‌وکار.
6. `Common` باید عمومی بماند و منطق feature-specific نداشته باشد.

## مالکیت فایل‌ها
- دامنه: `Entities/**`
- داده و migration: `Data/**`
- سرویس و قرارداد: `Services/**`
- کنترلرها: `API/Controllers/**`
- DI/Middleware/Auth wiring: `WebFramwork/**`
- ابزارها و exceptionهای مشترک: `Common/**`

## قوانین پیاده‌سازی
1. منطق کسب‌وکار داخل controller نوشته نشود.
2. validation نزدیک DTO/Service انجام شود.
3. از الگوهای DI موجود و Autofac فعلی تبعیت شود.
4. مدیریت خطا با exceptionهای موجود (`AppException` و ...).
5. mapping از مسیر AutoMapper فعلی انجام شود.
6. مقدار حساس در کد hardcode نشود.
7. قرارداد nullability واقعی و صادقانه باشد:
   - اگر مقدار ممکن است وجود نداشته باشد، نوع nullable (`?`) تعریف شود.
   - متد non-nullable نباید `null` برگرداند.
8. برای مدل‌های دامنه/EF:
   - propertyهای non-null باید با constructor/default/`required` مقداردهی اولیه شوند.
   - استفاده از `null!` فقط وقتی مجاز است که ORM/framework مقداردهی را تضمین کند (مثل navigation propertyهای EF) و باید حداقلی باشد.
9. برای reflection و metadata فریم‌ورک:
   - خروجی‌های nullable مثل `GetEntryAssembly()`، `EndpointMetadata` و collectionهای swagger حتماً guard شوند.
   - از cast مستقیم و unsafe از `object` پرهیز شود؛ از safe-cast و fallback استفاده شود.
10. قواعد پایداری JWT/Auth:
   - واحد زمان انقضای توکن باید شفاف و منطبق با تنظیمات باشد (دقیقه => `AddMinutes`).
   - در eventهای اعتبارسنجی توکن، `Fail` فقط در صورت شکست صریح شرط‌های اعتبارسنجی انجام شود.
11. قواعد داده‌سنگین (Indexing/Query Tuning):
   - برای queryهای لیستی/جست‌وجوی پرترافیک از `TagWith("Feature.Endpoint")` استفاده شود تا ردیابی و تحلیل آن‌ها در لاگ کندی کوئری ساده باشد.
   - برای endpointهای read-only، تا جای ممکن از `AsNoTracking` و projection به DTO استفاده شود.
   - در نقاطی که هنوز pattern نهایی بیزینس مشخص نیست، به‌جای حدس زدن ایندکس، `TODO(index)` دقیق کنار query/config ثبت شود.
   - ایندکس جدید فقط بعد از مشاهده الگوی واقعی مصرف + بررسی execution plan اضافه شود؛ از ایندکس‌گذاری حدسی و بی‌رویه پرهیز شود.
12. قواعد پایه Telemetry:
   - middleware مربوط به telemetry درخواست در محیط‌های غیرتستی فعال بماند، مگر با دلیل عملیاتی مشخص.
   - `TraceId` و `SpanId` در لاگ ساخت‌یافته حفظ شوند تا اتصال بین log، metric و slow-query ممکن باشد.
   - tagهای بیزینسی/KPI فقط بعد از تثبیت semantics دامنه اضافه شوند؛ از tagهای پر-cardinality و پرنویز پرهیز شود.

## سیاست کیفیت و Warning
1. هدف ریپو: بدون warning کامپایل (`0` warning).
2. هر warning جدید regression محسوب می‌شود و باید قبل از اتمام تسک رفع شود.
3. مخفی‌کردن warning با `#pragma` گسترده یا suppression سراسری بدون تایید صریح ممنوع است.
4. قبل از تحویل، گیت warning اجرا شود:
   - `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1`
5. به‌روزرسانی baseline فقط برای cleanup عمدی و بازبینی‌شده مجاز است:
   - `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1 -UpdateBaseline`

## Workflow انجام تسک
1. ابتدا `knowledge/**` و راهنمای بیزینسی مرتبط در `docs/business/**` را مرور کن.
2. لایه‌های درگیر را بخوان.
3. اثر معماری تغییر را بررسی کن.
4. کمترین تغییر موضعی را اعمال کن.
5. تغییرات DTO/Service/Controller را در لایه درست انجام بده.
6. اگر `Entities/**`، تنظیمات EF، فایل `Data/ApplicationDbContext.cs`، فایل‌های `Data/Migrations/**` یا `ApplicationDbContextModelSnapshot` تغییر کردند، قبل از نهایی‌سازی نسخه بعدی فایل در `database-documents/` ساخته شود.
7. اگر behavior بیزینسی یا رفتار API تغییر کرد، قبل از نهایی‌سازی نسخه بعدی فایل در `flow-documents/` ساخته شود.
8. در صورت امکان build/test اجرا کن.
9. خروجی نهایی شامل فایل‌های تغییر کرده + دلیل معماری + مستندات HTML اجباری ساخته‌شده باشد.

## Definition of Done
1. پروژه build شود.
2. مرزهای معماری نقض نشود.
3. behavior جدید از API قابل دسترسی باشد.
4. در تغییر مدل داده، migration لازم اضافه شود.
5. اگر شکل یا semantics دیتابیس تغییر کرد، نسخه بعدی فایل در `database-documents/` ساخته شده باشد.
6. اگر flow بیزینسی یا رفتار endpoint تغییر کرد، نسخه بعدی فایل در `flow-documents/` ساخته شده باشد.
7. warning جدید نسبت به baseline اضافه نشده باشد.
8. در تغییرات داده‌سنگین، قابلیت رصد کوئری حفظ شده باشد (Tagگذاری کوئری + لاگ کندی).

## قواعد اجباری نسخه‌بندی مستندات HTML
1. مستند تغییرات دیتابیس:
   - شرط اجرا: هر تغییر در `Entities/**`، تنظیمات EF، migrationها یا semantics دیتابیس.
   - مسیر خروجی: `database-documents/`
   - الگوی نام‌گذاری: `database-document-v{N}.html`
   - قانون نسخه‌بندی: فایل قبلی هرگز overwrite نشود و همیشه نسخه بعدی ساخته شود.
   - قانون عملیاتی:
     - آخرین شماره نسخه از فایل‌های موجود کشف و یک واحد افزایش داده شود.
     - اگر نسخه قبلی وجود نداشت، از `v1` شروع شود.
     - تا وقتی این فایل ساخته نشده، task کامل محسوب نشود.
     - در گزارش نهایی مسیر فایل ساخته‌شده صریحاً ذکر شود.

2. مستند تغییرات flow برنامه:
   - شرط اجرا: هر تغییر در flow بیزینسی، orchestration سرویس‌ها، رفتار endpointها یا قواعد کسب‌وکار.
   - مسیر خروجی: `flow-documents/`
   - الگوی نام‌گذاری: `flow-documents-v{N}.html`
   - قانون نسخه‌بندی: فایل قبلی هرگز overwrite نشود و همیشه نسخه بعدی ساخته شود.
   - قانون عملیاتی:
     - آخرین شماره نسخه از فایل‌های موجود کشف و یک واحد افزایش داده شود.
     - اگر نسخه قبلی وجود نداشت، از `v1` شروع شود.
     - تا وقتی این فایل ساخته نشده، task کامل محسوب نشود.
     - در گزارش نهایی مسیر فایل ساخته‌شده صریحاً ذکر شود.

## فرمان‌های پیشنهادی برای بررسی محلی
از ریشه پروژه اجرا شود:
- `dotnet restore`
- `dotnet build`
- `dotnet test` (اگر پروژه تست وجود دارد)
- `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1`

---
در تعارض بین این فایل و دستور مستقیم نگه‌دارنده پروژه، دستور نگه‌دارنده اولویت دارد.
