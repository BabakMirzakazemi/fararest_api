---
title: "AuthenticationService.GetCurrentBearerHash()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_getcurrentbearerhash"
label: ".GetCurrentBearerHash()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L794"
community: "0"
norm_label: ".getcurrentbearerhash()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.GetCurrentBearerHash()

- Category: `Services`
- Label: `.GetCurrentBearerHash()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L794`
- Graph Id: `authentications_authenticationservice_authenticationservice_getcurrentbearerhash`
- Community: `0`

depends_on:: [[AuthenticationService.ComputeSha256()]]
upstream:: [[AuthenticationService]], [[AuthenticationService.GetMySessionsAsync()]], [[AuthenticationService.RevokeOtherSessionsAsync()]]
downstream:: [[AuthenticationService.ComputeSha256()]]

## Dependencies
- [[AuthenticationService.ComputeSha256()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.ComputeSha256()]]

## Upstream Relationships
### Calls
- [[AuthenticationService.GetMySessionsAsync()]] -> `calls`
- [[AuthenticationService.RevokeOtherSessionsAsync()]] -> `calls`

### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

