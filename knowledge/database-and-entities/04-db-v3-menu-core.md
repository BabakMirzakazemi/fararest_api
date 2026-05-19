# 04-db-v3-menu-core.md

مستند دیتابیس Fararest API - نسخه v3 - Menu Management Core
مستند دیتابیس Fararest API
این نسخه نمای کلی مدل فعلی دیتابیس را حفظ می‌کند و تغییرات برجسته مربوط به Menu Management Core و موجودیت‌های پشتیبان Digital Menu را مستند می‌سازد.
نسخه
v3
تاریخ
2026-05-19
تغییر برجسته
افزودن و تثبیت هسته مدیریت منو
تغییرات این نسخه
افزودن جدول
menu
به‌عنوان ریشه منطقی ساختار منو برای هر سازمان/کسب‌وکار.
افزودن جدول
menu_category
با قابلیت سلسله‌مراتب والد/فرزند و اتصال به منو.
افزودن جدول
menu_item
برای آیتم‌های قابل فروش با شناسه یکتای
code
در سطح سازمان.
افزودن جدول‌های پایه مکمل شامل
menu_unit
،
menu_ingredient
،
menu_ingredient_component
و
menu_item_ingredient
.
افزودن جدول‌های حوزه Digital Menu شامل
menu_digital_menu
و جداول پروفایل، زمان‌بندی و بازدید.
اتکای تمام داده‌های منو به شناسه سازمان و استفاده منطقی از
accounts_organization
به‌عنوان ریشه مالکیت کسب‌وکار.
نمای کلی دامنه‌های دیتابیس
دامنه
نمونه جدول‌ها
نقش در سیستم
Identity / Users
AspNetUsers
،
AspNetRoles
،
AspNetUserRoles
احراز هویت پایه، نقش‌ها و سازوکار امنیتی Identity.
Accounts / Authorization
accounts_organization
،
accounts_membership
،
accounts_permission
،
accounts_role_permission
مدیریت سازمان، عضویت، مجوزها، نشست‌ها و کنترل امنیت سازمانی.
Licenses
licenses_plan
،
licenses_plan_price
،
licenses_subscription
تعریف پلن و اشتراک سرویس.
Payments
payments_wallet
،
payments_wallet_entry
،
payments_operation
کیف پول و عملیات مالی.
Menu / Digital Menu
menu
،
menu_category
،
menu_item
،
menu_unit
،
menu_ingredient
،
menu_digital_menu
مرجع اصلی ساختار منو، آیتم‌ها، مواد، و کانال نمایش دیجیتال.
CRM
crm_customer
،
crm_customer_note
،
crm_discount_campaign
داده‌های مشتری، تعاملات و کمپین‌ها.
Support
support_ticket
،
support_ticket_message
،
support_ticket_event
تیکت، پیام و رویدادهای پشتیبانی.
Episodic Memory
memory_episode
،
memory_episode_tag
،
memory_episode_reference
حافظه رویدادی durable برای agent و تیم.
موجودیت مرجع مالکیت سازمان
accounts_organization
فیلد
نوع
توضیح
id
bigint
شناسه یکتای سازمان/کسب‌وکار.
name
varchar(200)
نام تجاری سازمان.
tax_id
/
national_id
varchar(50)?
شناسه‌های رسمی سازمان.
is_active
boolean
وضعیت فعال بودن سازمان.
province_id
/
county_id
int?
موقعیت جغرافیایی.
address
/
postal_code
/
logo_url
text / varchar(20)?
اطلاعات نمایشی و ارتباطی سازمان.
داده‌های حوزه منو از نظر کسب‌وکاری باید به یک رکورد معتبر در این جدول متصل باشند، حتی اگر در schema فعلی همه روابط به‌صورت foreign key صریح مدل نشده باشند.
جزئیات هسته مدیریت منو
1) menu
فیلد
نوع
توضیح
id
bigint
شناسه منو.
organization_id
bigint
مالک سازمانی منو.
name
varchar(200)
نام منو.
description
text?
شرح اختیاری.
is_active
boolean
فعال/غیرفعال بودن منو.
created_at
/
updated_at
timestamptz
زمان ایجاد و آخرین بروزرسانی.
ایندکس‌های مهم:
IX_menu_organization_id
و
IX_menu_organization_id_name
.
2) menu_category
فیلد
نوع
توضیح
id
bigint
شناسه دسته‌بندی.
organization_id
bigint
دامنه سازمانی دسته‌بندی.
menu_id
bigint
منوی مالک.
parent_id
bigint?
والد اختیاری برای ساختار درختی.
name
varchar(200)
نام دسته‌بندی.
description
text?
توضیح اختیاری.
image_urls
text[]?
تصاویر نمایشی دسته.
is_active
boolean
وضعیت فعال/غیرفعال.
created_at
/
updated_at
timestamptz
ردیابی زمانی.
روابط مهم:
menu_category.menu_id -> menu.id
و self-reference روی
parent_id
با حذف restrict.
3) menu_item
فیلد
نوع
توضیح
id
bigint
شناسه آیتم.
organization_id
bigint
دامنه سازمانی آیتم.
category_id
bigint
دسته‌بندی مالک.
name
varchar(200)
نام آیتم قابل فروش.
code
varchar(100)
کد یکتای آیتم در سطح سازمان.
item_type
int
نوع آیتم مانند غذای تولیدی، نوشیدنی، پکیج یا افزودنی.
price_amount
numeric(18,2)
قیمت پایه.
description
text?
توضیح اختیاری.
image_urls
text[]?
تصاویر نمایشی.
is_active
boolean
وضعیت فعال/غیرفعال.
created_at
/
updated_at
timestamptz
ردیابی زمانی.
ایندکس مهم: یکتایی
(organization_id, code)
برای جلوگیری از کد تکراری در یک کسب‌وکار.
4) menu_unit
فیلد
نوع
توضیح
id
bigint
شناسه واحد.
code
varchar(50)
کد فنی واحد مانند
KG
یا
PCS
.
name
varchar(150)
نام خوانا برای واحد اندازه‌گیری.
is_active
boolean
وضعیت فعال بودن واحد.
جزئیات موجودیت‌های مکمل مواد اولیه و فرمول
جدول
فیلدهای کلیدی
کاربرد
menu_ingredient
organization_id
،
ingredient_type
،
code
،
unit_id
،
price_amount
تعریف مواد اولیه، محصول شرکتی یا اقلام پایه قابل مصرف/فروش.
menu_ingredient_component
prepared_ingredient_id
،
component_ingredient_id
،
quantity
،
unit_id
ساختار recipe-like برای مواد ترکیبی.
menu_item_ingredient
item_id
،
ingredient_id
،
quantity
،
unit_id
اتصال آیتم منو به مواد اولیه مصرفی.
جزئیات موجودیت‌های Digital Menu
جدول
کاربرد
نکات مهم
menu_digital_menu
تنظیمات اصلی منوی دیجیتال در سطح سازمان.
ایندکس یکتا روی
organization_id
و
token
.
menu_digital_menu_profile
پروفایل نمایشی، برندینگ، توضیحات و لینک‌های ارتباطی.
ایندکس یکتا روی
digital_menu_id
.
menu_digital_menu_schedule_weekly
زمان‌بندی هفتگی نمایش یا فعالیت منوی دیجیتال.
ایندکس یکتا روی
(digital_menu_id, weekday)
.
menu_digital_menu_schedule_exception
استثناهای تقویمی مثل تعطیلی یا تغییر ساعت در یک روز خاص.
ایندکس یکتا روی
(digital_menu_id, date_value)
.
menu_digital_menu_visit
ثبت بازدیدهای منوی دیجیتال برای تحلیل مصرف.
ایندکس روی
(digital_menu_id, visited_at)
.
menu_digital_menu_metrics
نمای تحلیلی read-only برای بازدید و شمارش دسته/آیتم.
به‌صورت
View
و بدون کلید اصلی مدل شده است.
روابط و قواعد کلیدی
هر
menu
به یک
organization_id
تعلق دارد و مبنای تمام دسته‌ها و آیتم‌های زیرمجموعه است.
هر
menu_category
دقیقاً به یک
menu
متصل است و می‌تواند والد اختیاری داشته باشد.
هر
menu_item
دقیقاً به یک
menu_category
متصل است.
کد آیتم در سطح سازمان یکتا نگه داشته می‌شود تا تداخل در فروش، گزارش و یکپارچگی رخ ندهد.
جدول‌های Digital Menu از نظر مالکیت به سازمان متصل‌اند و کانال نمایش مشتری‌محور را از داده مرجع منو جدا می‌کنند.
DDL خلاصه تغییرات برجسته این نسخه
accounts_organization
PK: id
Logical owner of menu data
menu
PK: id
IDX: organization_id
IDX: (organization_id, name)
menu_category
PK: id
FK: menu_id -> menu.id (Restrict)
FK: parent_id -> menu_category.id (Restrict)
menu_item
PK: id
FK: category_id -> menu_category.id
UQ: (organization_id, code)
menu_unit
PK: id
menu_ingredient
PK: id
UQ: (organization_id, code)
menu_ingredient_component
PK: id
UQ: (prepared_ingredient_id, component_ingredient_id)
menu_item_ingredient
PK: id
UQ: (item_id, ingredient_id)
menu_digital_menu
PK: id
UQ: organization_id
UQ: token