---
title: "OperationCatalogService"
type: "Service"
graph_id: "infrastructure_operationcatalogservice_operationcatalogservice"
label: "OperationCatalogService"
file_type: "code"
source_file: "AdminPanel/Infrastructure/OperationCatalogService.cs"
source_location: "L16"
community: "9"
norm_label: "operationcatalogservice"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# OperationCatalogService

- Category: `Services`
- Label: `OperationCatalogService`
- Source: `AdminPanel/Infrastructure/OperationCatalogService.cs`
- Location: `L16`
- Graph Id: `infrastructure_operationcatalogservice_operationcatalogservice`
- Community: `9`

depends_on:: [[ILogger]], [[IOperationCatalogService]], [[IReadOnlyList]], [[IServiceProvider]], [[JsonSerializerOptions]], [[OperationCatalogSecuritySettings (2)]]
upstream:: [[OperationCatalogService.cs]]
downstream:: [[ILogger]], [[IOperationCatalogService]], [[IReadOnlyList]], [[IServiceProvider]], [[JsonSerializerOptions]], [[OperationCatalogSecuritySettings (2)]], [[OperationCatalogService.DiscoverOperations()]], [[OperationCatalogService.GetOperation()]], [[OperationCatalogService.GetOperations()]], [[OperationCatalogService.InvokeAsync()]], [[OperationCatalogService.IsOperationAllowedByPolicy()]], [[OperationCatalogService.IsOperationSafeShape()]], [[OperationCatalogService.IsRequired()]], [[OperationCatalogService.IsServiceResolvable()]], [[OperationCatalogService.SerializeResult()]], [[OperationCatalogService.TryConvert()]]

## Dependencies
- [[ILogger]]
- [[IOperationCatalogService]]
- [[IReadOnlyList]]
- [[IServiceProvider]]
- [[JsonSerializerOptions]]
- [[OperationCatalogSecuritySettings (2)]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IOperationCatalogService]]

### Method
- `method` -> [[OperationCatalogService.DiscoverOperations()]]
- `method` -> [[OperationCatalogService.GetOperation()]]
- `method` -> [[OperationCatalogService.GetOperations()]]
- `method` -> [[OperationCatalogService.InvokeAsync()]]
- `method` -> [[OperationCatalogService.IsOperationAllowedByPolicy()]]
- `method` -> [[OperationCatalogService.IsOperationSafeShape()]]
- `method` -> [[OperationCatalogService.IsRequired()]]
- `method` -> [[OperationCatalogService.IsServiceResolvable()]]
- `method` -> [[OperationCatalogService.SerializeResult()]]
- `method` -> [[OperationCatalogService.TryConvert()]]

### References
- `references` -> [[ILogger]]
- `references` -> [[IReadOnlyList]]
- `references` -> [[IServiceProvider]]
- `references` -> [[JsonSerializerOptions]]
- `references` -> [[OperationCatalogSecuritySettings (2)]]

## Upstream Relationships
### Contains
- [[OperationCatalogService.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

