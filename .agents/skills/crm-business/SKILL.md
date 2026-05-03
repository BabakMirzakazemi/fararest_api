#  Business Skill

## Purpose
Use this skill when implementing crm business services, contracts, and endpoints.

## Source of Truth
- Primary doc: docs/business/crm.md
- Index: docs/business/README.md

## Implementation Checklist
1. Read and summarize business scope and rules from docs/business/crm.md.
2. Map use-cases into proper layers:
   - DTO/contracts in Services
   - business logic in Services/Services
   - persistence and EF config in Data/Entities
   - thin API handlers/controllers in API
3. Keep architecture boundaries from AGENTS.md.
4. Add/adjust migrations only when schema change is required.
5. Verify with build and warning baseline script.

## Output Expectations
- List changed files
- Explain layer placement
- State verification steps
- Mention risks/open questions
