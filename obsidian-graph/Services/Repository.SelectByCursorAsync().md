---
title: "Repository.SelectByCursorAsync()"
type: "Service"
graph_id: "repositories_repository_repository_selectbycursorasync"
label: ".SelectByCursorAsync()"
file_type: "code"
source_file: "Services/Services/Repositories/Repository.cs"
source_location: "L380"
community: "2"
norm_label: ".selectbycursorasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# Repository.SelectByCursorAsync()

- Category: `Services`
- Label: `.SelectByCursorAsync()`
- Source: `Services/Services/Repositories/Repository.cs`
- Location: `L380`
- Graph Id: `repositories_repository_repository_selectbycursorasync`
- Community: `2`

depends_on:: [[Repository.BuildSeekPredicate()]], [[Repository.EncodeCursorValue()]], [[Repository.NormalizeCursorPageSize()]], [[Repository.TryDecodeCursorValue()]]
upstream:: [[Repository (2)]]
downstream:: [[Repository.BuildSeekPredicate()]], [[Repository.EncodeCursorValue()]], [[Repository.NormalizeCursorPageSize()]], [[Repository.TryDecodeCursorValue()]]

## Dependencies
- [[Repository.BuildSeekPredicate()]]
- [[Repository.EncodeCursorValue()]]
- [[Repository.NormalizeCursorPageSize()]]
- [[Repository.TryDecodeCursorValue()]]

## Downstream Relationships
### Calls
- `calls` -> [[Repository.BuildSeekPredicate()]]
- `calls` -> [[Repository.EncodeCursorValue()]]
- `calls` -> [[Repository.NormalizeCursorPageSize()]]
- `calls` -> [[Repository.TryDecodeCursorValue()]]

## Upstream Relationships
### Method
- [[Repository (2)]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

