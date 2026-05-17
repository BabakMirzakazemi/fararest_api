# Episodic Memory Playbook

## هدف

این playbook مشخص می‌کند تیم و agentها چگونه از Episodic Memory استفاده کنند.

## مراحل ثبت episode

1. رویداد را classify کنید.
2. عنوان کوتاه و summary دقیق بنویسید.
3. `OccurredAtUtc` واقعی را ثبت کنید.
4. tagهای معنادار و کم‌تعداد اضافه کنید.
5. referenceهای پایدار مانند module، table، migration، endpoint و file را اضافه کنید.
6. اگر رویداد follow-up است، `ParentEpisodeId` را ثبت کنید.

## اتوماسیون فعلی

- startup دیتابیس وقتی migration pending داشته باشد، episode خودکار ثبت می‌کند.
- اجرای `--bootstrap-db` حتی بدون migration pending هم می‌تواند رویداد bootstrap ثبت کند.
- exceptionهای unhandled در API به‌عنوان incident ثبت می‌شوند.
- برای تصمیم‌های معماری، bug fixهای کاربرمحور و تغییرات feature، agent یا developer هنوز باید record/search را آگاهانه صدا بزند.

## الگوهای پیشنهادی tag

- `auth`
- `migration`
- `incident`
- `performance`
- `security`
- `deployment`
- `crm`
- `payments`
- `menu`
- `agent`

## چه چیزهایی episode نیستند

- TODOهای روزمره کم‌اهمیت
- logهای تکراری runtime
- اقداماتی که هیچ outcome پایداری ندارند

## جستجو قبل از اقدام

قبل از تغییر مهم این پرسش‌ها را بررسی کنید:

- آیا قبلا همین باگ یا migration ثبت شده؟
- آیا یک تلاش ناموفق مرتبط وجود دارد؟
- آیا decision قبلی این ماژول را محدود کرده؟
- آیا incident یا finding امنیتی مرتبط با این area وجود دارد؟
