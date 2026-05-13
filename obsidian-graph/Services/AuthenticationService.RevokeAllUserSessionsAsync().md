---
title: "AuthenticationService.RevokeAllUserSessionsAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_revokeallusersessionsasync"
label: ".RevokeAllUserSessionsAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L756"
community: "0"
norm_label: ".revokeallusersessionsasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.RevokeAllUserSessionsAsync()

- Category: `Services`
- Label: `.RevokeAllUserSessionsAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L756`
- Graph Id: `authentications_authenticationservice_authenticationservice_revokeallusersessionsasync`
- Community: `0`

depends_on:: [[AuthenticationService.MapUserIdToLegacyInt()]]
upstream:: [[AuthenticationService]], [[AuthenticationService.ResetPasswordWithOtpAsync()]]
downstream:: [[AuthenticationService.MapUserIdToLegacyInt()]]

## Dependencies
- [[AuthenticationService.MapUserIdToLegacyInt()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.MapUserIdToLegacyInt()]]

## Upstream Relationships
### Calls
- [[AuthenticationService.ResetPasswordWithOtpAsync()]] -> `calls`

### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

