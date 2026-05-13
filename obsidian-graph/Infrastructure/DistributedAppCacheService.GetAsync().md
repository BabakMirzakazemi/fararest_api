---
title: "DistributedAppCacheService.GetAsync()"
type: "Infrastructure"
graph_id: "caching_distributedappcacheservice_distributedappcacheservice_getasync"
label: ".GetAsync()"
file_type: "code"
source_file: "WebFramwork/Infrastructure/Caching/DistributedAppCacheService.cs"
source_location: "L18"
community: "8"
norm_label: ".getasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# DistributedAppCacheService.GetAsync()

- Category: `Infrastructure`
- Label: `.GetAsync()`
- Source: `WebFramwork/Infrastructure/Caching/DistributedAppCacheService.cs`
- Location: `L18`
- Graph Id: `caching_distributedappcacheservice_distributedappcacheservice_getasync`
- Community: `8`

depends_on:: [[DistributedAppCacheService.IsInFailureCooldown()]], [[DistributedAppCacheService.MarkBackendUnavailable()]], [[DistributedAppCacheService.RemoveAsync()]]
upstream:: [[DistributedAppCacheService]]
downstream:: [[DistributedAppCacheService.IsInFailureCooldown()]], [[DistributedAppCacheService.MarkBackendUnavailable()]], [[DistributedAppCacheService.RemoveAsync()]]

## Dependencies
- [[DistributedAppCacheService.IsInFailureCooldown()]]
- [[DistributedAppCacheService.MarkBackendUnavailable()]]
- [[DistributedAppCacheService.RemoveAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[DistributedAppCacheService.IsInFailureCooldown()]]
- `calls` -> [[DistributedAppCacheService.MarkBackendUnavailable()]]
- `calls` -> [[DistributedAppCacheService.RemoveAsync()]]

## Upstream Relationships
### Method
- [[DistributedAppCacheService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

