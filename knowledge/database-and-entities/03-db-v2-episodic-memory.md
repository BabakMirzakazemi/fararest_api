# 03-db-v2-episodic-memory.md

مستند دیتابیس Fararest API - نسخه v2 - Episodic Memory
مستند دیتابیس Fararest API
این نسخه نمای کلی مدل دیتابیس فعلی را به‌همراه تغییرات نسخه اپیزودیک مموری مستند می‌کند.
نسخه
v2
تاریخ
2026-05-17
تغییر برجسته
افزودن Episodic Memory
تغییرات این نسخه
افزودن جدول
memory_episode
برای ثبت رویدادهای زمان‌مند و durable پروژه.
افزودن جدول
memory_episode_tag
برای tagهای قابل جستجو.
افزودن جدول
memory_episode_reference
برای referenceهای پایدار به ماژول، فایل، endpoint، migration و جدول.
حفظ portability در سطح EF schema و پرهیز از قابلیت‌های provider-specific برای این feature.
نمای کلی دامنه‌های دیتابیس
دامنه
نمونه جدول‌ها
کاربرد
Identity / Users
AspNetUsers
،
AspNetRoles
،
AspNetUserRoles
،
ConfirmationCode
احراز هویت، نقش‌ها، تایید ثبت‌نام و امنیت پایه کاربران
Accounts / Authorization
accounts_permission
،
accounts_role_permission
،
accounts_user_session
،
accounts_membership
مجوزها، session، عضویت سازمانی، نقش و grant/revoke
Licenses
licenses_plan
،
licenses_plan_price
،
licenses_subscription
مدیریت plan، قیمت‌گذاری و subscription
Payments
payments_wallet
،
payments_wallet_entry
،
payments_operation
کیف پول و عملیات مالی
Menu / Digital Menu
menu_item
،
menu_category
،
menu_digital_menu
،
menu_ingredient
منو، دسته‌بندی، آیتم‌ها و منوی دیجیتال
CRM
crm_customer
،
crm_customer_note
،
crm_discount_campaign
مدیریت مشتری، وفاداری، کمپین و coupon
Support
support_ticket
،
support_ticket_message
،
support_ticket_event
تیکت، پیام و eventهای پشتیبانی
Episodic Memory
memory_episode
،
memory_episode_tag
،
memory_episode_reference
حافظه رویدادی durable برای تصمیم‌ها، incidentها، migrationها و یافته‌ها
جزئیات جداول جدید Episodic Memory
1) memory_episode
ستون
نوع
توضیح
id
uuid
شناسه اصلی episode
type
int
نوع رویداد مانند decision، bug، migration، incident
importance
int
اولویت و شدت اهمیت
source
int
منبع رویداد: انسان، agent، system، pipeline
status
int?
وضعیت اختیاری رویداد
title
varchar(200)
عنوان کوتاه و قابل جستجو
summary
varchar(1000)
خلاصه رویداد
details
text
جزئیات کامل رویداد
occurred_at_utc
timestamptz
زمان واقعی وقوع
recorded_at_utc
timestamptz
زمان ثبت در سیستم
actor_id
/
actor_name
varchar
عامل انسانی یا ماشینی مرتبط با رویداد
correlation_id
varchar(128)
اتصال به trace/request/deployment context
environment
varchar(64)
محیط اجرا مانند dev/staging/prod
commit_sha
varchar(64)
اتصال به commit کد
deduplication_key
varchar(256)
کلید جلوگیری از ثبت duplicate
metadata_json
text
اطلاعات تکمیلی اختیاری به‌صورت provider-neutral
parent_episode_id
uuid?
ارتباط follow-up با episode قبلی
2) memory_episode_tag
ستون
نوع
توضیح
id
bigint
شناسه tag
episode_id
uuid
ارجاع به episode
tag
varchar(64)
tag نرمال‌شده و قابل جستجو
3) memory_episode_reference
ستون
نوع
توضیح
id
bigint
شناسه reference
episode_id
uuid
ارجاع به episode
type
int
نوع reference مانند module، table، migration، endpoint
reference_key
varchar(256)
کلید پایدار برای lookup
reference_label
varchar(256)
label خواناتر برای نمایش
ایندکس‌ها و ملاحظات portability
ایندکس روی
occurred_at_utc
،
(type, occurred_at_utc)
و
(importance, occurred_at_utc)
برای queryهای recent/important/search.
ایندکس روی
tag
و
(type, reference_key)
برای جستجوی سریع tag/reference.
از
jsonb
، array، trigger، function و GIN index برای این feature استفاده نشده است.
طراحی جدول‌ها و typeها برای مهاجرت آینده به SQL Server مانع جدی ایجاد نمی‌کند.
DDL خلاصه Episodic Memory
memory_episode
PK: id
FK: parent_episode_id -> memory_episode.id (Restrict)
memory_episode_tag
PK: id
FK: episode_id -> memory_episode.id (Cascade)
UQ: (episode_id, tag)
memory_episode_reference
PK: id
FK: episode_id -> memory_episode.id (Cascade)
UQ: (episode_id, type, reference_key)