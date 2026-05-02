# adminpanel-review-checklist.md

Use this checklist for AdminPanel tooling changes.

1. Is heavy logic in `Infrastructure` and not in PageModel?
2. Are `OperationCatalogService` conversions and error messages still predictable?
3. Are entity key token parse/build rules unchanged and symmetric?
4. Are FK dropdown options still generated with useful labels?
5. Was DI registration updated for any new dependency?
6. Were build commands for AdminPanel (and solution if needed) attempted?
