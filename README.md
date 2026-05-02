# babak_backend_api

## AI Agent Rules (VS Code + GPT-5.3-Codex)
This repository uses instruction files for AI coding agents:

- Global rules (EN): `AGENTS.md`
- Global rules (FA): `AGENTS.fa.md`
- Copilot repository rules (EN): `.github/copilot-instructions.md`
- Copilot repository rules (FA): `.github/copilot-instructions.fa.md`
- Path-specific rules (EN + FA): `.github/instructions/*.instructions.md`
- Workspace chat settings: `.vscode/settings.json`

### Quick check in VS Code
1. Open Command Palette and run `Chat: Open Chat Customizations`.
2. Confirm `AGENTS.md`, `AGENTS.fa.md`, `.github/copilot-instructions.md`, and `.github/instructions/*` are detected.
3. In Chat Diagnostics, verify active instruction files for your current task.

### Notes
- `AGENTS.md` and `AGENTS.fa.md` define clean/onion boundaries and task workflow.
- Path-specific instruction files add extra layer-focused rules for controllers, services, and domain/data.
- If instructions conflict, task-level maintainer instruction has priority.

## Codex Customization (Applied)
- Hierarchical instructions:
  - Root: `AGENTS.md`
  - Local overrides: `AdminPanel/AGENTS.override.md`, `Services/AGENTS.override.md`
- VS Code setting enabled: `chat.useNestedAgentsMdFiles = true`
- Repository skills:
  - `.agents/skills/dotnet-backend-auth/SKILL.md`
  - `.agents/skills/adminpanel-ops/SKILL.md`
- Skill UI metadata:
  - `.agents/skills/dotnet-backend-auth/agents/openai.yaml`
  - `.agents/skills/adminpanel-ops/agents/openai.yaml`
- Skill reference checklist:
  - `.agents/skills/dotnet-backend-auth/references/team-auth-checklist.md`
  - `.agents/skills/adminpanel-ops/references/adminpanel-review-checklist.md`

## Warning Gate (Staged Quality)
- Script: `tools/Check-WarningBaseline.ps1`
- Plan doc: `tools/Nullability-Quality-Staged-Plan.md`
- Check for new warnings:
  - `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1`
- Refresh baseline after intentional cleanup:
  - `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1 -UpdateBaseline`

