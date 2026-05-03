param(
    [string]$StartupProject = "API/API.csproj",
    [string]$DataProject = "Data/Data.csproj"
)

$ErrorActionPreference = "Stop"

Write-Host "[1/2] Running EF migrations (Update-Database)..."
dotnet ef database update --project $DataProject --startup-project $StartupProject
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "[2/2] Running bootstrap (SQL objects + data initializers)..."
dotnet run --project $StartupProject -- --bootstrap-db
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Database initialization completed successfully."

