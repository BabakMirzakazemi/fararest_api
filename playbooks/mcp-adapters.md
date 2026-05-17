# MCP Adapter Playbook

## هدف

این playbook مشخص می‌کند agentها در این پروژه چگونه از adapterهای MCP استفاده کنند.

## اولویت فعلی

1. اول از episodic memory search استفاده کنید.
2. اگر وضعیت runtime یا دیتابیس مهم بود، از `agent.status.database` و `agent.status.memory` استفاده کنید.
3. فقط بعد از بررسی context کافی، episode جدید ثبت کنید.

## Adapterهای مناسب در وضعیت فعلی

- `episodes.search`
- `episodes.get`
- `episodes.recent`
- `episodes.record`
- `agent.status.database`
- `agent.status.memory`

## Adapterهایی که فعلا نباید اضافه شوند

- direct PostgreSQL query tool
- write-capable database admin tool
- broad shell-like operational tool بدون محدودیت دامنه

## اصل طراحی

- read-only first
- application boundary first
- low-noise and auditable
- provider-portable
