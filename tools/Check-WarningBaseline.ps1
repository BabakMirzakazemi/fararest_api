[CmdletBinding()]
param(
    # Entry project used to calculate the current warning set.
    [string]$Project = "API/API.csproj",

    # Baseline snapshot (normalized warnings).
    [string]$BaselinePath = "tools/warnings-baseline.txt",

    # Full raw build output for troubleshooting.
    [string]$LogPath = "tools/warnings-last-build.log",

    # Regenerates baseline from current warnings after intentional cleanup work.
    [switch]$UpdateBaseline
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Normalize-WarningLine {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Line,

        [Parameter(Mandatory = $true)]
        [string]$RepoRoot
    )

    $normalized = $Line.Trim()

    # Keep baseline portable by replacing absolute repo path.
    $normalized = $normalized.Replace($RepoRoot, ".")

    # Ignore line/column shifts so harmless refactors don't churn the baseline.
    $normalized = [regex]::Replace($normalized, "\(\d+,\d+\)", "(:)")

    # Remove trailing project path wrapper which varies by build entrypoint.
    $normalized = [regex]::Replace($normalized, "\s+\[[^\]]+\.csproj\]", "")

    # Normalize separators and spaces to keep comparison deterministic.
    $normalized = $normalized.Replace("\\", "/")
    $normalized = [regex]::Replace($normalized, "\s+", " ").Trim()

    return $normalized
}

$repoRoot = (Resolve-Path ".").Path
$resolvedProject = (Resolve-Path $Project).Path
$resolvedBaseline = Join-Path $repoRoot $BaselinePath
$resolvedLog = Join-Path $repoRoot $LogPath

$logDir = Split-Path -Parent $resolvedLog
if (-not [string]::IsNullOrWhiteSpace($logDir)) {
    New-Item -ItemType Directory -Force -Path $logDir | Out-Null
}

# Keep dotnet home local to repo to avoid machine-specific side effects.
$env:DOTNET_CLI_HOME = (Resolve-Path ".dotnet_home").Path

$buildOutput = & dotnet build $resolvedProject -t:Rebuild -m:1 -v minimal -p:NuGetAudit=false 2>&1
$buildOutputText = $buildOutput | ForEach-Object { $_.ToString() }
$buildOutputText | Set-Content -Encoding UTF8 $resolvedLog

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. See $resolvedLog" -ForegroundColor Red
    exit $LASTEXITCODE
}

$warningLines = @($buildOutputText | Where-Object { $_ -match ": warning " })
$normalizedWarnings = @(
    $warningLines |
        ForEach-Object { Normalize-WarningLine -Line $_ -RepoRoot $repoRoot } |
        Sort-Object -Unique
)

if ($UpdateBaseline) {
    $baselineDir = Split-Path -Parent $resolvedBaseline
    if (-not [string]::IsNullOrWhiteSpace($baselineDir)) {
        New-Item -ItemType Directory -Force -Path $baselineDir | Out-Null
    }

    if ($normalizedWarnings.Count -eq 0) {
        Set-Content -Encoding UTF8 -Path $resolvedBaseline -Value ""
    }
    else {
        $normalizedWarnings | Set-Content -Encoding UTF8 $resolvedBaseline
    }
    Write-Host "Warning baseline updated: $resolvedBaseline" -ForegroundColor Green
    Write-Host "Tracked warnings: $($normalizedWarnings.Count)"
    exit 0
}

if (-not (Test-Path $resolvedBaseline)) {
    Write-Host "Baseline not found: $resolvedBaseline" -ForegroundColor Red
    Write-Host "Run once with -UpdateBaseline to initialize." -ForegroundColor Yellow
    exit 3
}

$baselineWarnings = @(
    Get-Content $resolvedBaseline |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        Sort-Object -Unique
)

$baselineSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
foreach ($entry in $baselineWarnings) {
    [void]$baselineSet.Add($entry)
}

$currentSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
foreach ($entry in $normalizedWarnings) {
    [void]$currentSet.Add($entry)
}

$newWarnings = @()
foreach ($entry in $normalizedWarnings) {
    if (-not $baselineSet.Contains($entry)) {
        $newWarnings += $entry
    }
}

$resolvedWarnings = @()
foreach ($entry in $baselineWarnings) {
    if (-not $currentSet.Contains($entry)) {
        $resolvedWarnings += $entry
    }
}

Write-Host "Current warnings (unique): $($normalizedWarnings.Count)"
Write-Host "Baseline warnings (unique): $($baselineWarnings.Count)"

if ($resolvedWarnings.Count -gt 0) {
    Write-Host "Resolved warnings since baseline: $($resolvedWarnings.Count)" -ForegroundColor Green
}

if ($newWarnings.Count -gt 0) {
    Write-Host "New warnings introduced: $($newWarnings.Count)" -ForegroundColor Red
    $newWarnings | ForEach-Object { Write-Host " - $_" -ForegroundColor Red }
    exit 2
}

Write-Host "No new warnings introduced compared to baseline." -ForegroundColor Green
exit 0
