---
name: episodic-memory
description: Use for tasks that should search or record durable project/event memory in this repository, especially before migrations, auth/security changes, repeated fixes, incidents, or deployment-adjacent work.
---

# Episodic Memory Skill

Use this skill when the task benefits from durable project memory or should create it.

## Scope

- Domain: `Entities/EpisodicMemory/**`
- Application contracts/services/DTOs: `Services/**/EpisodicMemory/**`
- Persistence: `Data/Configurations/EpisodicMemory/**`, `Data/Migrations/**`
- API: `API/Controllers/Admin/v1/EpisodesController.cs`
- Docs and playbooks:
  - `agent.md`
  - `memory.md`
  - `decision-log.md`
  - `playbooks/episodic-memory.md`

## When To Search Episodes First

- Before changing database schema or migrations
- Before auth/security changes
- Before retrying a bug fix or workaround
- Before modifying unstable modules
- Before operational or deployment-sensitive actions

## When To Record A New Episode

- Architectural decision
- Bug discovered
- Bug fixed
- Migration added/applied
- Incident / unexpected production-style failure
- Failed attempt worth not repeating
- Deployment event
- Performance or security finding

## Required Episode Quality

- Short, explicit title
- Searchable summary
- Real event time in UTC
- Low-noise tags
- Stable references:
  - module
  - entity
  - table
  - migration id
  - endpoint
  - file
  - commit sha

## Automation Notes

- Backend now records some system episodes automatically:
  - unhandled API exceptions
  - startup migration/bootstrap events
- Agents should still actively call episode search/record flows for user-driven engineering work.

## Verification

- `dotnet build babak_base.slnx`
- `dotnet test babak_base.slnx`
- `powershell -ExecutionPolicy Bypass -File .\tools\Check-WarningBaseline.ps1`
- `python -m graphify update .`
