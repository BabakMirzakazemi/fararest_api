---
title: "AuthorizationDataInitializer.InitializeData()"
type: "Infrastructure"
graph_id: "datainitializer_authorizationdatainitializer_authorizationdatainitializer_initializedata"
label: ".InitializeData()"
file_type: "code"
source_file: "Services/DataInitializer/AuthorizationDataInitializer.cs"
source_location: "L19"
community: "21"
norm_label: ".initializedata()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthorizationDataInitializer.InitializeData()

- Category: `Infrastructure`
- Label: `.InitializeData()`
- Source: `Services/DataInitializer/AuthorizationDataInitializer.cs`
- Location: `L19`
- Graph Id: `datainitializer_authorizationdatainitializer_authorizationdatainitializer_initializedata`
- Community: `21`

depends_on:: [[AuthorizationDataInitializer.EnsurePlan()]], [[AuthorizationDataInitializer.EnsureSeedUserPlan()]], [[AuthorizationDataInitializer.MapPlanPermissions()]], [[AuthorizationDataInitializer.MapRolePermissions()]]
upstream:: [[AuthorizationDataInitializer]]
downstream:: [[AuthorizationDataInitializer.EnsurePlan()]], [[AuthorizationDataInitializer.EnsureSeedUserPlan()]], [[AuthorizationDataInitializer.MapPlanPermissions()]], [[AuthorizationDataInitializer.MapRolePermissions()]]

## Dependencies
- [[AuthorizationDataInitializer.EnsurePlan()]]
- [[AuthorizationDataInitializer.EnsureSeedUserPlan()]]
- [[AuthorizationDataInitializer.MapPlanPermissions()]]
- [[AuthorizationDataInitializer.MapRolePermissions()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthorizationDataInitializer.EnsurePlan()]]
- `calls` -> [[AuthorizationDataInitializer.EnsureSeedUserPlan()]]
- `calls` -> [[AuthorizationDataInitializer.MapPlanPermissions()]]
- `calls` -> [[AuthorizationDataInitializer.MapRolePermissions()]]

## Upstream Relationships
### Method
- [[AuthorizationDataInitializer]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

