---
title: "NoOpAppCacheService"
type: "Service"
graph_id: "infrastructure_noopappcacheservice_noopappcacheservice"
label: "NoOpAppCacheService"
file_type: "code"
source_file: "AdminPanel/Infrastructure/NoOpAppCacheService.cs"
source_location: "L7"
community: "8"
norm_label: "noopappcacheservice"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# NoOpAppCacheService

- Category: `Services`
- Label: `NoOpAppCacheService`
- Source: `AdminPanel/Infrastructure/NoOpAppCacheService.cs`
- Location: `L7`
- Graph Id: `infrastructure_noopappcacheservice_noopappcacheservice`
- Community: `8`

depends_on:: [[IAppCacheService]]
upstream:: [[NoOpAppCacheService.cs]]
downstream:: [[IAppCacheService]], [[NoOpAppCacheService.GetAsync()]], [[NoOpAppCacheService.RemoveAsync()]], [[NoOpAppCacheService.SetAsync()]]

## Dependencies
- [[IAppCacheService]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IAppCacheService]]

### Method
- `method` -> [[NoOpAppCacheService.GetAsync()]]
- `method` -> [[NoOpAppCacheService.RemoveAsync()]]
- `method` -> [[NoOpAppCacheService.SetAsync()]]

## Upstream Relationships
### Contains
- [[NoOpAppCacheService.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

