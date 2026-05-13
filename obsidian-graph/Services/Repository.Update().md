---
title: "Repository.Update()"
type: "Service"
graph_id: "repositories_repository_repository_update"
label: ".Update()"
file_type: "code"
source_file: "Services/Services/Repositories/Repository.cs"
source_location: "L206"
community: "2"
norm_label: ".update()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# Repository.Update()

- Category: `Services`
- Label: `.Update()`
- Source: `Services/Services/Repositories/Repository.cs`
- Location: `L206`
- Graph Id: `repositories_repository_repository_update`
- Community: `2`

depends_on:: [[Repository.SaveChanges()]]
upstream:: [[Repository (2)]], [[Repository.UpdateAsync()]]
downstream:: [[Repository.SaveChanges()]]

## Dependencies
- [[Repository.SaveChanges()]]

## Downstream Relationships
### Calls
- `calls` -> [[Repository.SaveChanges()]]

## Upstream Relationships
### Calls
- [[Repository.UpdateAsync()]] -> `calls`

### Method
- [[Repository (2)]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

