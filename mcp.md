# MCP Notes

## Current Recommendation

در وضعیت فعلی پروژه، MCP باید کوچک، امن و read-only-first باشد.

## Recommended Adapters

- `episodes.search`
- `episodes.get`
- `episodes.recent`
- `episodes.record`
- `agent.status.database`
- `agent.status.memory`

## Why These First

- روی APIهای موجود سوار می‌شوند و business logic را دور نمی‌زنند.
- به agent برای جستجو در memory و فهمیدن وضعیت runtime کمک می‌کنند.
- با PostgreSQL فعلی کار می‌کنند ولی portability را به SQL Server خراب نمی‌کنند.

## Direct PostgreSQL MCP

فعلا توصیه نمی‌شود.

دلایل:

- agent را از boundaryهای application عبور می‌دهد
- امنیت و کنترل دسترسی را سخت‌تر می‌کند
- portability را تضعیف می‌کند
- ممکن است queryهای ad-hoc و کم‌قانون تولید کند

اگر در آینده نیاز واقعی ثابت شد، فقط read-only و فقط برای سناریوهای تشخیصی محدود بررسی شود.
