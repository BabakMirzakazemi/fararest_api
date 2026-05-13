---
title: "DistributedAppCacheService.RemoveAsync()"
type: "Infrastructure"
graph_id: "caching_distributedappcacheservice_distributedappcacheservice_removeasync"
label: ".RemoveAsync()"
file_type: "code"
source_file: "WebFramwork/Infrastructure/Caching/DistributedAppCacheService.cs"
source_location: "L72"
community: "8"
norm_label: ".removeasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# DistributedAppCacheService.RemoveAsync()

- Category: `Infrastructure`
- Label: `.RemoveAsync()`
- Source: `WebFramwork/Infrastructure/Caching/DistributedAppCacheService.cs`
- Location: `L72`
- Graph Id: `caching_distributedappcacheservice_distributedappcacheservice_removeasync`
- Community: `8`

depends_on:: [[DistributedAppCacheService.IsInFailureCooldown()]], [[DistributedAppCacheService.MarkBackendUnavailable()]]
upstream:: [[DistributedAppCacheService]], [[DistributedAppCacheService.GetAsync()]]
downstream:: [[DistributedAppCacheService.IsInFailureCooldown()]], [[DistributedAppCacheService.MarkBackendUnavailable()]]

## Dependencies
- [[DistributedAppCacheService.IsInFailureCooldown()]]
- [[DistributedAppCacheService.MarkBackendUnavailable()]]

## Downstream Relationships
### Calls
- `calls` -> [[DistributedAppCacheService.IsInFailureCooldown()]]
- `calls` -> [[DistributedAppCacheService.MarkBackendUnavailable()]]

## Upstream Relationships
### Calls
- [[DistributedAppCacheService.GetAsync()]] -> `calls`

### Method
- [[DistributedAppCacheService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

