---
title: "EntityAdminService.ParseKeyToken()"
type: "Infrastructure"
graph_id: "infrastructure_entityadminservice_entityadminservice_parsekeytoken"
label: ".ParseKeyToken()"
file_type: "code"
source_file: "AdminPanel/Infrastructure/EntityAdminService.cs"
source_location: "L382"
community: "5"
norm_label: ".parsekeytoken()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# EntityAdminService.ParseKeyToken()

- Category: `Infrastructure`
- Label: `.ParseKeyToken()`
- Source: `AdminPanel/Infrastructure/EntityAdminService.cs`
- Location: `L382`
- Graph Id: `infrastructure_entityadminservice_entityadminservice_parsekeytoken`
- Community: `5`

depends_on:: [[EntityAdminService.TryConvert()]]
upstream:: [[EntityAdminService]], [[EntityAdminService.DeleteAsync()]], [[EntityAdminService.FindByKey()]], [[EntityAdminService.SaveAsync()]]
downstream:: [[EntityAdminService.TryConvert()]]

## Dependencies
- [[EntityAdminService.TryConvert()]]

## Downstream Relationships
### Calls
- `calls` -> [[EntityAdminService.TryConvert()]]

## Upstream Relationships
### Calls
- [[EntityAdminService.DeleteAsync()]] -> `calls`
- [[EntityAdminService.FindByKey()]] -> `calls`
- [[EntityAdminService.SaveAsync()]] -> `calls`

### Method
- [[EntityAdminService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

