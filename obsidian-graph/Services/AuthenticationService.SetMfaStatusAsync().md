---
title: "AuthenticationService.SetMfaStatusAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_setmfastatusasync"
label: ".SetMfaStatusAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L516"
community: "0"
norm_label: ".setmfastatusasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.SetMfaStatusAsync()

- Category: `Services`
- Label: `.SetMfaStatusAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L516`
- Graph Id: `authentications_authenticationservice_authenticationservice_setmfastatusasync`
- Community: `0`

depends_on:: [[AuthenticationService.EnsureMfaFeatureAsync()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.MapUserIdToLegacyInt()]]
upstream:: [[AuthenticationService]]
downstream:: [[AuthenticationService.EnsureMfaFeatureAsync()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.MapUserIdToLegacyInt()]]

## Dependencies
- [[AuthenticationService.EnsureMfaFeatureAsync()]]
- [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- [[AuthenticationService.MapUserIdToLegacyInt()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.EnsureMfaFeatureAsync()]]
- `calls` -> [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- `calls` -> [[AuthenticationService.MapUserIdToLegacyInt()]]

## Upstream Relationships
### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

