---
title: "UserService"
type: "Service"
graph_id: "authentications_userservice_userservice"
label: "UserService"
file_type: "code"
source_file: "Services/Services/Authentications/UserService.cs"
source_location: "L15"
community: "1"
norm_label: "userservice"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# UserService

- Category: `Services`
- Label: `UserService`
- Source: `Services/Services/Authentications/UserService.cs`
- Location: `L15`
- Graph Id: `authentications_userservice_userservice`
- Community: `1`

depends_on:: [[CachingSettings (2)]], [[IAppCacheService]], [[IMapper]], [[int]], [[IScopedDependency (2)]], [[IUserContext (2)]], [[IUserService (2)]], [[Repository]], [[string]]
upstream:: [[UserService.cs]]
downstream:: [[CachingSettings (2)]], [[IAppCacheService]], [[IMapper]], [[int]], [[IScopedDependency (2)]], [[IUserContext (2)]], [[IUserService (2)]], [[Repository]], [[string]], [[UserService.AddAsync()]], [[UserService.All()]], [[UserService.AllByCursor()]], [[UserService.BuildUsersCursorListCacheKey()]], [[UserService.BuildUsersListCacheKey()]], [[UserService.DeleteAsync()]], [[UserService.DeleteByIdAsync()]], [[UserService.GetUsersListCacheVersionAsync()]], [[UserService.RefreshUsersListCacheVersionAsync()]], [[UserService.UpdateAsync()]]

## Dependencies
- [[CachingSettings (2)]]
- [[IAppCacheService]]
- [[IMapper]]
- [[int]]
- [[IScopedDependency (2)]]
- [[IUserContext (2)]]
- [[IUserService (2)]]
- [[Repository]]
- [[string]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IScopedDependency (2)]]
- `inherits` -> [[IUserService (2)]]
- `inherits` -> [[Repository]]

### Method
- `method` -> [[UserService.AddAsync()]]
- `method` -> [[UserService.All()]]
- `method` -> [[UserService.AllByCursor()]]
- `method` -> [[UserService.BuildUsersCursorListCacheKey()]]
- `method` -> [[UserService.BuildUsersListCacheKey()]]
- `method` -> [[UserService.DeleteAsync()]]
- `method` -> [[UserService.DeleteByIdAsync()]]
- `method` -> [[UserService.GetUsersListCacheVersionAsync()]]
- `method` -> [[UserService.RefreshUsersListCacheVersionAsync()]]
- `method` -> [[UserService.UpdateAsync()]]

### References
- `references` -> [[CachingSettings (2)]]
- `references` -> [[IAppCacheService]]
- `references` -> [[IMapper]]
- `references` -> [[int]]
- `references` -> [[IUserContext (2)]]
- `references` -> [[string]]

## Upstream Relationships
### Contains
- [[UserService.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

