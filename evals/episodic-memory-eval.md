# Episodic Memory Eval

## سوال‌های ارزیابی

- آیا یک عامل قبل از migration، episodeهای مشابه را جستجو می‌کند؟
- آیا episodeهای ثبت‌شده title و summary قابل فهم دارند؟
- آیا referenceها پایدار و قابل match با Graphify هستند؟
- آیا duplicate episodeها به‌درستی رد می‌شوند؟
- آیا episodeهای مهم در queryهای `Important` و `Recent` پیدا می‌شوند؟

## سناریوهای حداقلی

1. ثبت architectural decision با reference به service و endpoint.
2. ثبت bug fix با parent episode از bug discovered.
3. جستجو بر اساس tag.
4. جستجو بر اساس migration id.
5. جستجو بر اساس commit sha.
6. رد شدن duplicate در بازه زمانی تعریف‌شده.

## معیار پذیرش

- ثبت episode موفق باشد.
- بازیابی single episode کامل باشد.
- search و paging درست کار کند.
- schema provider-specific نشود.
