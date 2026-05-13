---
title: "DistributedAppCacheService"
type: "Service"
graph_id: "caching_distributedappcacheservice_distributedappcacheservice"
label: "DistributedAppCacheService"
file_type: "code"
source_file: "WebFramwork/Infrastructure/Caching/DistributedAppCacheService.cs"
source_location: "L9"
community: "8"
norm_label: "distributedappcacheservice"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# DistributedAppCacheService

- Category: `Services`
- Label: `DistributedAppCacheService`
- Source: `WebFramwork/Infrastructure/Caching/DistributedAppCacheService.cs`
- Location: `L9`
- Graph Id: `caching_distributedappcacheservice_distributedappcacheservice`
- Community: `8`

depends_on:: [[IAppCacheService]], [[int]], [[JsonSerializerOptions]], [[long]]
upstream:: [[DistributedAppCacheService.cs]]
downstream:: [[DistributedAppCacheService.GetAsync()]], [[DistributedAppCacheService.IsInFailureCooldown()]], [[DistributedAppCacheService.MarkBackendUnavailable()]], [[DistributedAppCacheService.RemoveAsync()]], [[DistributedAppCacheService.SetAsync()]], [[IAppCacheService]], [[int]], [[JsonSerializerOptions]], [[long]]

## Dependencies
- [[IAppCacheService]]
- [[int]]
- [[JsonSerializerOptions]]
- [[long]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IAppCacheService]]

### Method
- `method` -> [[DistributedAppCacheService.GetAsync()]]
- `method` -> [[DistributedAppCacheService.IsInFailureCooldown()]]
- `method` -> [[DistributedAppCacheService.MarkBackendUnavailable()]]
- `method` -> [[DistributedAppCacheService.RemoveAsync()]]
- `method` -> [[DistributedAppCacheService.SetAsync()]]

### References
- `references` -> [[int]]
- `references` -> [[JsonSerializerOptions]]
- `references` -> [[long]]

## Upstream Relationships
### Contains
- [[DistributedAppCacheService.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

