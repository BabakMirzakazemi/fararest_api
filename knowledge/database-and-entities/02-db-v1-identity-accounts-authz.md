# 02-db-v1-identity-accounts-authz.md

﻿
مستند کامل دیتابیس Fararest API - Identity + Accounts + Authorization
مستند کامل دیتابیس Fararest API
پوشش: جداول Identity دات‌نت + جداول حوزه
accounts_*
+ مدل جدید Authorization.
Schema Focus
Identity + Accounts + AuthZ
Migration
20260506140425_AuthzPermissionsModel
فهرست
جداول Identity دات‌نت
جداول حوزه Accounts
جداول Authorization جدید
1) جداول Identity دات‌نت
جدول: AspNetUsers
کاربرد کلی جدول:
موجودیت اصلی کاربر سیستم (هویت، امنیت، وضعیت حساب، اطلاعات ورود).
فیلد
نوع
Nullable
کلید/رابطه
توضیح روشن
مثال کاربردی
Id
uuid
خیر
PK
شناسه یکتای کاربر برای تمام ارتباطات امنیتی.
ثبت ورود و نشست‌های کاربر با این شناسه.
UserName
varchar
بله
Index
نام کاربری لاگین یا شناسه نمایشی.
admin.main
Email
varchar
بله
Index
ایمیل کاربر برای لاگین/بازیابی.
ارسال لینک فعال‌سازی ایمیل.
PasswordHash
text
بله
-
هش رمز عبور (نه رمز خام).
اعتبارسنجی رمز در زمان login.
SecurityStamp
text
بله
-
برای invalidate کردن توکن‌های قدیمی.
بعد از تغییر رمز، توکن‌های قبلی نامعتبر شوند.
IsActive
bool
خیر
-
فعال/غیرفعال بودن حساب.
کاربر غیرفعال اجازه ورود ندارد.
جدول: AspNetRoles
کاربرد کلی جدول:
نگهداری نقش‌های کلان سیستم (User/Admin/SuperAdmin و ...).
فیلد
نوع
Nullable
کلید/رابطه
توضیح روشن
مثال کاربردی
Id
uuid
خیر
PK
شناسه یکتای نقش.
اتصال در جدول
AspNetUserRoles
.
Name
varchar
خیر
Unique
نام نقش قابل استفاده در authorize.
SuperAdmin
Description
varchar
خیر
-
توضیح معنای کسب‌وکاری نقش.
«مدیر کل سامانه»
جدول: AspNetUserRoles
کاربرد کلی جدول:
اتصال چند-به-چند کاربر و نقش.
فیلد
نوع
Nullable
کلید/رابطه
توضیح روشن
مثال کاربردی
UserId
uuid
خیر
FK → AspNetUsers
کاربری که نقش می‌گیرد.
کاربر به نقش Admin وصل می‌شود.
RoleId
uuid
خیر
FK → AspNetRoles
نقش تخصیص‌داده‌شده.
افزودن دسترسی مدیریتی.
جدول‌ها: AspNetUserClaims / AspNetRoleClaims / AspNetUserLogins / AspNetUserTokens / AspNetUserClaims
کاربرد کلی جدول:
نگهداری claim، لاگین خارجی، و توکن‌های امنیتی/فنی Identity.
جدول
کاربرد روشن
مثال کاربردی
AspNetUserClaims
Claimهای مستقیم کاربر.
Claim سفارشی برای tenant.
AspNetRoleClaims
Claimهای متصل به نقش.
Claim سطح سازمانی روی Admin.
AspNetUserLogins
نگهداری provider خارجی.
ورود گوگل/مایکروسافت.
AspNetUserTokens
توکن‌های داخلی Identity.
توکن reset password.
2) جداول حوزه Accounts (accounts_*)
این بخش جدول‌های عملیاتی عضویت، امنیت، OTP، نشست و تنظیمات امنیتی را پوشش می‌دهد.
جدول: accounts_organization
کاربرد کلی جدول:
اطلاعات پایه سازمان/کسب‌وکار مالک سرویس.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
id
bigint
خیر
شناسه سازمان.
مرجع خرید لایسنس و کیف پول.
name
varchar(200)
خیر
نام تجاری سازمان.
«رستوران مرکزی»
owner_user_id
int?
بله
شناسه مالک اولیه سازمان در مدل legacy.
مالک اولیه در زمان ثبت‌نام.
جدول: accounts_membership
کاربرد کلی جدول:
عضویت کاربر در سازمان و وضعیت فعال/مالک بودن.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
organization_id
bigint
خیر
سازمان هدف عضویت.
عضو شعبه تهران.
user_id
int
خیر
شناسه کاربر (پل legacy).
کاربر صندوق.
is_owner
bool
خیر
مالک بودن کاربر در سازمان.
امکان مدیریت سطح بالا.
جدول: accounts_membership_groups
کاربرد کلی جدول:
اتصال عضویت به گروه/نقش سازمانی.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
membership_id
bigint
خیر
عضویت هدف.
عضویت کاربر در سازمان الف.
group_id
int
خیر
گروه تخصیصی.
گروه «حسابداری».
جدول: accounts_membership_permissions
کاربرد کلی جدول:
Grant/deny مجوز در سطح عضویت سازمانی.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
membership_id
bigint
خیر
عضویت هدف.
کاربر سازمان الف.
permission_id
int
خیر
مجوزی که override می‌شود.
عدم دسترسی به delete.
is_granted
bool
خیر
اعطا/عدم اعطا.
false
برای محدودسازی.
جدول: accounts_user_verification
کاربرد کلی جدول:
مدیریت OTP و تاییدیه‌های امنیتی کاربر.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
channel
varchar(20)
خیر
کانال ارسال کد (sms/email).
ارسال OTP پیامکی.
purpose
varchar(50)
خیر
هدف OTP.
reset_password
otp_code_hash
char(64)
خیر
هش کد یکبار مصرف.
جلوگیری از ذخیره کد خام.
expires_at
timestamptz
خیر
انقضای کد.
اعتبار 2 دقیقه‌ای.
جدول: accounts_user_session
کاربرد کلی جدول:
مدیریت نشست‌های فعال کاربر و revoke جلسه.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
session_public_id
uuid
خیر
شناسه عمومی جلسه.
نمایش در لیست MySessions.
session_secret_hash
char(64)
خیر
اثر انگشت توکن.
خروج از یک دستگاه خاص.
revoked_at
timestamptz
بله
زمان ابطال.
پس از reset password.
جدول: accounts_security_feature
کاربرد کلی جدول:
کاتالوگ قابلیت‌های امنیتی (مثل MFA).
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
code
varchar(80)
خیر
کلید فنی فیچر.
MFA_REQUIRED
default_enabled
bool
خیر
وضعیت پیش‌فرض.
MFA پیش‌فرض خاموش.
جدول: accounts_organization_security_setting
کاربرد کلی جدول:
فعال/غیرفعال‌سازی فیچر امنیتی در سطح سازمان.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
organization_id
bigint
خیر
سازمان هدف.
اجباری شدن MFA برای یک سازمان.
feature_id
bigint
خیر
فیچر امنیتی.
اتصال به MFA.
is_enabled
bool
خیر
وضعیت فعال.
مقدار true.
جدول: accounts_user_security_setting
کاربرد کلی جدول:
تنظیم امنیتی per-user برای ویژگی‌های امنیتی.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
user_id
int
خیر
کاربر هدف.
فعال‌سازی MFA برای یک کاربر خاص.
feature_id
bigint
خیر
فیچر امنیتی.
MFA_REQUIRED
is_enabled
bool
خیر
وضعیت فعال.
true/false
جدول: accounts_user_totp_factor
کاربرد کلی جدول:
نگهداری اطلاعات عامل TOTP برای MFA.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
secret_encrypted
bytea
خیر
کلید TOTP به‌شکل رمزنگاری‌شده.
اتصال اپ Google Authenticator.
status
varchar(20)
خیر
وضعیت عامل (active/disabled).
disabled بعد از لغو MFA.
جدول: accounts_user_oauth_identity
کاربرد کلی جدول:
لینک هویت OAuth خارجی به کاربر داخلی.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
provider
varchar(30)
خیر
ارائه‌دهنده هویت.
google
provider_subject
varchar(255)
خیر
شناسه یکتای کاربر در provider.
sub از Google ID token.
last_login_at
timestamptz
بله
آخرین ورود OAuth.
تحلیل رفتار ورود.
3) جداول Authorization جدید
جدول: accounts_permission
کاربرد کلی جدول:
مرجع مرکزی کلیدهای permission برای route/menu/module action.
فیلد
نوع
Nullable
کلید/قید
توضیح روشن
مثال کاربردی
key
varchar(120)
خیر
UNIQUE
کلید فنی بررسی دسترسی.
home.view
,
accounts.update
category
varchar(80)
خیر
-
گروه‌بندی منطقی مجوزها.
menu
,
crm
جدول: accounts_role_permission
کاربرد کلی جدول:
نگاشت پیش‌فرض مجوزهای نقش‌ها.
فیلد
نوع
Nullable
کلید/قید
توضیح روشن
مثال کاربردی
role_id + permission_id
uuid + bigint
خیر
UNIQUE مرکب
جلوگیری از اتصال تکراری نقش و مجوز.
نقش User دارای home.view.
جدول: accounts_plan_permission
کاربرد کلی جدول:
نگاشت مجوزها بر اساس پلن خریداری‌شده.
فیلد
نوع
Nullable
کلید/قید
توضیح روشن
مثال کاربردی
plan_id + permission_id
bigint + bigint
خیر
UNIQUE مرکب
هر پلن، مجموعه مجوز استاندارد خود را دارد.
پلن PRO دارای payments.view.
جدول: accounts_user_permission_grant
کاربرد کلی جدول:
اعطای مستقیم permission به کاربر (override مثبت).
فیلد
نوع
Nullable
کلید/قید
توضیح روشن
مثال کاربردی
user_id + permission_id
uuid + bigint
خیر
UNIQUE مرکب
یک grant تکراری برای کاربر ثبت نشود.
اعطای crm.view به یک User.
source
varchar(40)
خیر
-
منبع grant برای audit.
manual توسط SuperAdmin.
جدول: accounts_user_permission_revoke
کاربرد کلی جدول:
حذف مستقیم permission از کاربر (override منفی).
فیلد
نوع
Nullable
کلید/قید
توضیح روشن
مثال کاربردی
user_id + permission_id
uuid + bigint
خیر
UNIQUE مرکب
Revocation تکراری ثبت نشود.
حذف accounts.delete از کاربر خاص.
جدول: accounts_user_plan_subscription
کاربرد کلی جدول:
تعیین پلن فعال هر کاربر و بازه زمانی اثرگذاری آن در محاسبه دسترسی.
فیلد
نوع
Nullable
توضیح روشن
مثال کاربردی
is_active
boolean
خیر
فعال/غیرفعال بودن اشتراک پلن کاربر.
غیرفعال‌سازی پلن پس از انقضا.
starts_at / ends_at
timestamptz
ends_at بله
کنترل بازه زمانی معتبر بودن پلن.
دسترسی Pro فقط تا تاریخ انقضا.