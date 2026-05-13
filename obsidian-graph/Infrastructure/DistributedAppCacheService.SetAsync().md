---
title: "DistributedAppCacheService.SetAsync()"
type: "Infrastructure"
graph_id: "caching_distributedappcacheservice_distributedappcacheservice_setasync"
label: ".SetAsync()"
file_type: "code"
source_file: "WebFramwork/Infrastructure/Caching/DistributedAppCacheService.cs"
source_location: "L51"
community: "8"
norm_label: ".setasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# DistributedAppCacheService.SetAsync()

- Category: `Infrastructure`
- Label: `.SetAsync()`
- Source: `WebFramwork/Infrastructure/Caching/DistributedAppCacheService.cs`
- Location: `L51`
- Graph Id: `caching_distributedappcacheservice_distributedappcacheservice_setasync`
- Community: `8`

depends_on:: [[DistributedAppCacheService.IsInFailureCooldown()]], [[DistributedAppCacheService.MarkBackendUnavailable()]]
upstream:: [[DistributedAppCacheService]]
downstream:: [[DistributedAppCacheService.IsInFailureCooldown()]], [[DistributedAppCacheService.MarkBackendUnavailable()]]

## Dependencies
- [[DistributedAppCacheService.IsInFailureCooldown()]]
- [[DistributedAppCacheService.MarkBackendUnavailable()]]

## Downstream Relationships
### Calls
- `calls` -> [[DistributedAppCacheService.IsInFailureCooldown()]]
- `calls` -> [[DistributedAppCacheService.MarkBackendUnavailable()]]

## Upstream Relationships
### Method
- [[DistributedAppCacheService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

