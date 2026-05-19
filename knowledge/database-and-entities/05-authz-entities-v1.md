# 05-authz-entities-v1.md

﻿
مستند موجودیت‌های دیتابیس Fararest API - نسخه Authorization
مستند موجودیت‌های دیتابیس Fararest API
این نسخه برای پروژه جاری
fararest_api
تهیه شده و مدل جدید Authorization (Permission-Based + Plan-Based + User Override) را پوشش می‌دهد.
پروژه
ASP.NET Core + EF Core + Identity
نسخه مهاجرت
20260506140425_AuthzPermissionsModel
مسیر خروجی
database-documents
فهرست
نمای کلی
هسته هویتی (Identity + Auth)
مدل جدید Authorization
ارتباط پلن و دسترسی
منطق دسترسی موثر
نمای کلی
در معماری جدید، دسترسی موثر کاربر بر اساس اجتماع نقش، پلن و Grant مستقیم و سپس حذف Revocation محاسبه می‌شود.
effective_permissions = (role_permissions UNION plan_permissions UNION user_grants) - user_revokes
ERD متنی
AspNetRoles (1) --< accounts_role_permission (N) >-- (1) accounts_permission
licenses_plan (1) --< accounts_plan_permission (N) >-- (1) accounts_permission
AspNetUsers (1) --< accounts_user_permission_grant (N) >-- (1) accounts_permission
AspNetUsers (1) --< accounts_user_permission_revoke (N) >-- (1) accounts_permission
AspNetUsers (1) --< accounts_user_plan_subscription (N) >-- (1) licenses_plan
هسته هویتی (Identity + Auth)
موجودیت‌های پایه احراز هویت و نشست کاربر که در پروژه فعال هستند:
جدول: AspNetUsers
فیلد
نوع
Nullable
توضیح
Id
uuid
خیر
شناسه یکتای کاربر.
UserName / Email / Mobile
string
بله/خیر بسته به فیلد
هویت ورود و اطلاعات تماس کاربر.
IsActive
bool
خیر
وضعیت فعال بودن حساب.
SecurityStamp
string
بله
برای اعتبارسنجی توکن و invalidation امنیتی.
جدول: accounts_user_session
فیلد
نوع
Nullable
توضیح
session_public_id
uuid
خیر
شناسه عمومی نشست.
session_secret_hash
char(64)
خیر
هش توکن برای revoke/list (بدون نگهداری توکن خام).
auth_method
varchar(30)
خیر
روش ورود (password/otp/...)
issued_at / expires_at / revoked_at
timestamp with tz
revoked_at بله
چرخه عمر نشست.
مدل جدید Authorization
جدول: accounts_permission
فیلد
نوع
Nullable
کلید/قید
توضیح
id
bigint identity
خیر
PK
شناسه مجوز.
key
varchar(120)
خیر
UNIQUE
کلید فنی مجوز مثل
home.view
.
name
varchar(200)
خیر
-
نام نمایشی مجوز.
category
varchar(80)
خیر
-
دسته مجوز (menu/accounts/licenses/...)
is_active
boolean
خیر
-
فعال/غیرفعال بودن مجوز.
جدول: accounts_role_permission
فیلد
نوع
Nullable
کلید/قید
توضیح
role_id
uuid
خیر
UQ(role_id, permission_id)
ارجاع به نقش Identity.
permission_id
bigint
خیر
UQ(role_id, permission_id)
ارجاع به مجوز.
updated_by_user_id
uuid
بله
-
ردپای تغییر توسط ادمین.
created_at / updated_at
timestamp with tz
خیر
-
Audit زمانی.
جدول: accounts_user_permission_grant
فیلد
نوع
Nullable
کلید/قید
توضیح
user_id
uuid
خیر
UQ(user_id, permission_id)
کاربر هدف.
permission_id
bigint
خیر
UQ(user_id, permission_id)
مجوزی که مستقیم اعطا شده.
source
varchar(40)
خیر
-
منبع اعطا (manual/system/...)
notes
varchar(500)
بله
-
توضیح مدیریتی.
جدول: accounts_user_permission_revoke
فیلد
نوع
Nullable
کلید/قید
توضیح
user_id
uuid
خیر
UQ(user_id, permission_id)
کاربر هدف.
permission_id
bigint
خیر
UQ(user_id, permission_id)
مجوزی که explicitly revoke شده.
notes
varchar(500)
بله
-
علت revoke.
ارتباط پلن و دسترسی
جدول: accounts_plan_permission
فیلد
نوع
Nullable
کلید/قید
توضیح
plan_id
bigint
خیر
UQ(plan_id, permission_id)
پلن لایسنس.
permission_id
bigint
خیر
UQ(plan_id, permission_id)
مجوز مرتبط با پلن.
updated_by_user_id
uuid
بله
-
ردپای تغییر.
جدول: accounts_user_plan_subscription
فیلد
نوع
Nullable
توضیح
user_id
uuid
خیر
مالک اشتراک کاربری.
plan_id
bigint
خیر
پلن فعال/غیرفعال شده برای کاربر.
is_active
boolean
خیر
وضعیت فعال.
starts_at / ends_at
timestamp with tz
ends_at بله
بازه اثرگذاری پلن.
پلن‌های baseline seeded:
BASIC
,
PRO
,
ENTERPRISE
.
منطق دسترسی موثر و API
Endpoint عملیاتی سمت فرانت:
GET /api/v1/Auth/MyAuthorization
{
"userId": "guid",
"roles": ["User", "Admin"],
"permissions": ["home.view", "menu.home", "accounts.view"],
"permissionVersion": "16-hex"
}
سرویس محاسبه:
EffectiveAuthorizationService
با cache کوتاه‌مدت و version-based cache key.
سرویس مدیریت SuperAdmin: تغییر mappingهای Role/Plan و Grant/Revoke کاربر با ثبت فیلدهای audit.