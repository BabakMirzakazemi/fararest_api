# Tooling Notes

## Build / Verify

- `dotnet build babak_base.slnx`
- `dotnet test babak_base.slnx`
- `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1`

## EF Core

- add migration:
  - `dotnet ef migrations add <Name> --project Data\Data.csproj --startup-project API\API.csproj`
- remove migration:
  - `dotnet ef migrations remove --project Data\Data.csproj --startup-project API\API.csproj`

## Graphify

- refresh graph:
  - `python -m graphify update .`

## Episodic Memory APIs

- `POST /api/admin/v1/Episodes/RecordAsync`
- `GET /api/admin/v1/Episodes/GetAsync?id=<guid>`
- `POST /api/admin/v1/Episodes/SearchAsync`
- `GET /api/admin/v1/Episodes/RecentAsync`
- `GET /api/admin/v1/Episodes/ImportantAsync`
