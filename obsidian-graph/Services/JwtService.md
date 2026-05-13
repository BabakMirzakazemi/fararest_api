---
title: "JwtService"
type: "Service"
graph_id: "authentications_jwtservice_jwtservice"
label: "JwtService"
file_type: "code"
source_file: "Services/Services/Authentications/JwtService.cs"
source_location: "L9"
community: "0"
norm_label: "jwtservice"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# JwtService

- Category: `Services`
- Label: `JwtService`
- Source: `Services/Services/Authentications/JwtService.cs`
- Location: `L9`
- Graph Id: `authentications_jwtservice_jwtservice`
- Community: `0`

depends_on:: [[IJwtService (2)]], [[IScopedDependency (2)]], [[SignInManager]], [[SiteSettings (2)]]
upstream:: [[JwtService.cs]]
downstream:: [[IJwtService (2)]], [[IScopedDependency (2)]], [[JwtService.GenerateAsync()]], [[JwtService.GetClaimsAsync()]], [[JwtService.Validate()]], [[SignInManager]], [[SiteSettings (2)]]

## Dependencies
- [[IJwtService (2)]]
- [[IScopedDependency (2)]]
- [[SignInManager]]
- [[SiteSettings (2)]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IJwtService (2)]]
- `inherits` -> [[IScopedDependency (2)]]

### Method
- `method` -> [[JwtService.GenerateAsync()]]
- `method` -> [[JwtService.GetClaimsAsync()]]
- `method` -> [[JwtService.Validate()]]

### References
- `references` -> [[SignInManager]]
- `references` -> [[SiteSettings (2)]]

## Upstream Relationships
### Contains
- [[JwtService.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

