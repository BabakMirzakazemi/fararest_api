---
title: "Repository.Delete()"
type: "Service"
graph_id: "repositories_repository_repository_delete"
label: ".Delete()"
file_type: "code"
source_file: "Services/Services/Repositories/Repository.cs"
source_location: "L222"
community: "2"
norm_label: ".delete()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# Repository.Delete()

- Category: `Services`
- Label: `.Delete()`
- Source: `Services/Services/Repositories/Repository.cs`
- Location: `L222`
- Graph Id: `repositories_repository_repository_delete`
- Community: `2`

depends_on:: [[Repository.SaveChanges()]]
upstream:: [[Repository (2)]], [[Repository.DeleteById()]]
downstream:: [[Repository.SaveChanges()]]

## Dependencies
- [[Repository.SaveChanges()]]

## Downstream Relationships
### Calls
- `calls` -> [[Repository.SaveChanges()]]

## Upstream Relationships
### Calls
- [[Repository.DeleteById()]] -> `calls`

### Method
- [[Repository (2)]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

