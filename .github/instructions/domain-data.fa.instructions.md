---
applyTo: "Data/**/*.cs,Entities/**/*.cs"
description: "قواعد فارسی دامنه و داده"
---
# قواعد Domain/Data
- `Entities` مستقل از API و WebFramwork بماند.
- جزئیات EF Core در `Data` باشد.
- تغییر مدل داده با migration همگام شود.
- مدل persistence مستقیم به API نشت نکند.
