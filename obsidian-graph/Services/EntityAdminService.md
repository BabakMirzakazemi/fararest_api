---
title: "EntityAdminService"
type: "Service"
graph_id: "infrastructure_entityadminservice_entityadminservice"
label: "EntityAdminService"
file_type: "code"
source_file: "AdminPanel/Infrastructure/EntityAdminService.cs"
source_location: "L25"
community: "5"
norm_label: "entityadminservice"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# EntityAdminService

- Category: `Services`
- Label: `EntityAdminService`
- Source: `AdminPanel/Infrastructure/EntityAdminService.cs`
- Location: `L25`
- Graph Id: `infrastructure_entityadminservice_entityadminservice`
- Community: `5`

depends_on:: [[ApplicationDbContext]], [[IEntityAdminService]], [[IReadOnlyList]], [[MethodInfo]], [[string]], [[Type]]
upstream:: [[EntityAdminService.cs]]
downstream:: [[ApplicationDbContext]], [[EntityAdminService.ApplyDefaultOrder()]], [[EntityAdminService.ApplySearch()]], [[EntityAdminService.AsNoTracking()]], [[EntityAdminService.BuildEntityDescriptors()]], [[EntityAdminService.BuildKeyToken()]], [[EntityAdminService.BuildLookupLabel()]], [[EntityAdminService.Count()]], [[EntityAdminService.CreatePropertyDescriptor()]], [[EntityAdminService.DeleteAsync()]], [[EntityAdminService.FindAsync()]], [[EntityAdminService.FindByKey()]], [[EntityAdminService.FormatValue()]], [[EntityAdminService.GetForeignKeyOptionsAsync()]], [[EntityAdminService.GetManagedEntities()]], [[EntityAdminService.GetManagedEntity()]], [[EntityAdminService.GetPage()]], [[EntityAdminService.GetQueryable()]], [[EntityAdminService.GuessDisplayProperty()]], [[EntityAdminService.IsEditable()]], [[EntityAdminService.IsSimpleType()]], [[EntityAdminService.ParseKeyToken()]], [[EntityAdminService.SaveAsync()]], [[EntityAdminService.TryConvert()]], [[IEntityAdminService]], [[IReadOnlyList]], [[MethodInfo]], [[string]], [[Type]]

## Dependencies
- [[ApplicationDbContext]]
- [[IEntityAdminService]]
- [[IReadOnlyList]]
- [[MethodInfo]]
- [[string]]
- [[Type]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IEntityAdminService]]

### Method
- `method` -> [[EntityAdminService.ApplyDefaultOrder()]]
- `method` -> [[EntityAdminService.ApplySearch()]]
- `method` -> [[EntityAdminService.AsNoTracking()]]
- `method` -> [[EntityAdminService.BuildEntityDescriptors()]]
- `method` -> [[EntityAdminService.BuildKeyToken()]]
- `method` -> [[EntityAdminService.BuildLookupLabel()]]
- `method` -> [[EntityAdminService.Count()]]
- `method` -> [[EntityAdminService.CreatePropertyDescriptor()]]
- `method` -> [[EntityAdminService.DeleteAsync()]]
- `method` -> [[EntityAdminService.FindAsync()]]
- `method` -> [[EntityAdminService.FindByKey()]]
- `method` -> [[EntityAdminService.FormatValue()]]
- `method` -> [[EntityAdminService.GetForeignKeyOptionsAsync()]]
- `method` -> [[EntityAdminService.GetManagedEntities()]]
- `method` -> [[EntityAdminService.GetManagedEntity()]]
- `method` -> [[EntityAdminService.GetPage()]]
- `method` -> [[EntityAdminService.GetQueryable()]]
- `method` -> [[EntityAdminService.GuessDisplayProperty()]]
- `method` -> [[EntityAdminService.IsEditable()]]
- `method` -> [[EntityAdminService.IsSimpleType()]]
- `method` -> [[EntityAdminService.ParseKeyToken()]]
- `method` -> [[EntityAdminService.SaveAsync()]]
- `method` -> [[EntityAdminService.TryConvert()]]

### References
- `references` -> [[ApplicationDbContext]]
- `references` -> [[IReadOnlyList]]
- `references` -> [[MethodInfo]]
- `references` -> [[string]]
- `references` -> [[Type]]

## Upstream Relationships
### Contains
- [[EntityAdminService.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

