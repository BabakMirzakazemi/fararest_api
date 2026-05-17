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

## قواعد کیفیت episode

- title باید صریح و کوتاه باشد، نه مبهم و کلی.
- summary باید به‌اندازه کافی context بدهد تا بدون باز کردن details هم قابل فهم باشد.
- برای episodeهای مهم یا تصمیم/مهاجرت/bug fix، حداقل یک reference پایدار ثبت کنید.
- tagها کم‌تعداد و معنادار بمانند؛ over-tagging کیفیت retrieval را پایین می‌آورد.

## اتوماسیون فعلی

- startup دیتابیس وقتی migration pending داشته باشد، episode خودکار ثبت می‌کند.
- اجرای `--bootstrap-db` حتی بدون migration pending هم می‌تواند رویداد bootstrap ثبت کند.
- exceptionهای unhandled در API به‌عنوان incident ثبت می‌شوند.
- برای تصمیم‌های معماری، bug fixهای کاربرمحور و تغییرات feature، agent یا developer هنوز باید record/search را آگاهانه صدا بزند.

## Hybrid Search سبک

- جستجو حالا می‌تواند علاوه بر filter، از hybrid ranking سبک استفاده کند.
- سیگنال‌های ranking:
  - match روی title/summary/details
  - tag boost
  - reference boost
  - importance
  - recency
- این لایه هنوز relational و provider-portable است و به vector store وابسته نیست.

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

## workflow پیشنهادی agent

1. اگر کار حساس است، `AgentTools/DatabaseStatusAsync` و `AgentTools/MemoryStatusAsync` را بررسی کنید.
2. قبل از تغییر، `Episodes/SearchAsync` را با hybrid ranking اجرا کنید.
3. بعد از outcome مهم، `Episodes/RecordAsync` را صدا بزنید.
4. اگر queryهای تکراری کیفیت نامشخص دارند، `Episodes/EvaluateSearchAsync` را برای سنجش recall/precision اجرا کنید.
