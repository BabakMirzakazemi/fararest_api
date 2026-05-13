---
title: "SimpleRateLimitMiddleware"
type: "Infrastructure"
graph_id: "middlewares_simpleratelimitmiddleware_simpleratelimitmiddleware"
label: "SimpleRateLimitMiddleware"
file_type: "code"
source_file: "WebFramwork/Middlewares/SimpleRateLimitMiddleware.cs"
source_location: "L8"
community: "16"
norm_label: "simpleratelimitmiddleware"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# SimpleRateLimitMiddleware

- Category: `Infrastructure`
- Label: `SimpleRateLimitMiddleware`
- Source: `WebFramwork/Middlewares/SimpleRateLimitMiddleware.cs`
- Location: `L8`
- Graph Id: `middlewares_simpleratelimitmiddleware_simpleratelimitmiddleware`
- Community: `16`

depends_on:: [[ConcurrentDictionary]]
upstream:: [[SimpleRateLimitMiddleware.cs]]
downstream:: [[ConcurrentDictionary]], [[SimpleRateLimitMiddleware.BuildClientKey()]], [[SimpleRateLimitMiddleware.InvokeAsync()]]

## Dependencies
- [[ConcurrentDictionary]]

## Downstream Relationships
### Method
- `method` -> [[SimpleRateLimitMiddleware.BuildClientKey()]]
- `method` -> [[SimpleRateLimitMiddleware.InvokeAsync()]]

### References
- `references` -> [[ConcurrentDictionary]]

## Upstream Relationships
### Contains
- [[SimpleRateLimitMiddleware.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

