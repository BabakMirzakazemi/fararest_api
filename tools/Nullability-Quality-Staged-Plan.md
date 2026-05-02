# Nullability and Quality Staged Plan

This repository now uses a warning baseline gate to prevent **new** compile/analyzer warnings while we gradually fix existing debt.

## Why staged?

- Current codebase has legacy warnings from multiple layers.
- Fixing all warnings at once is high-risk and slows feature delivery.
- Staged cleanup gives safe progress with measurable quality improvement.

## Workflow

1. Check current status against baseline:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1
```

2. If intentional warning cleanup was completed, refresh baseline:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1 -UpdateBaseline
```

3. If new warnings appear, fix them before merge.

## Suggested cleanup order

1. `Common` nullability warnings (`CS8618`, `CS8625`, `CS8603`)
2. `WebFramwork` API/result nullability warnings
3. `Services` repository nullability warnings
4. Entity navigation/property contracts (`required`, nullable refs)
5. Turn selected warning groups into errors per project

## CI recommendation

Run this command in CI for pull requests:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1
```

If output contains "New warnings introduced", fail the pipeline.
