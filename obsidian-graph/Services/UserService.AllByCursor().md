---
title: "UserService.AllByCursor()"
type: "Service"
graph_id: "authentications_userservice_userservice_allbycursor"
label: ".AllByCursor()"
file_type: "code"
source_file: "Services/Services/Authentications/UserService.cs"
source_location: "L87"
community: "1"
norm_label: ".allbycursor()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# UserService.AllByCursor()

- Category: `Services`
- Label: `.AllByCursor()`
- Source: `Services/Services/Authentications/UserService.cs`
- Location: `L87`
- Graph Id: `authentications_userservice_userservice_allbycursor`
- Community: `1`

depends_on:: [[UserService.BuildUsersCursorListCacheKey()]], [[UserService.GetUsersListCacheVersionAsync()]]
upstream:: [[UserService]]
downstream:: [[UserService.BuildUsersCursorListCacheKey()]], [[UserService.GetUsersListCacheVersionAsync()]]

## Dependencies
- [[UserService.BuildUsersCursorListCacheKey()]]
- [[UserService.GetUsersListCacheVersionAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[UserService.BuildUsersCursorListCacheKey()]]
- `calls` -> [[UserService.GetUsersListCacheVersionAsync()]]

## Upstream Relationships
### Method
- [[UserService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

