---
title: "AuthenticationService.ConfirmEmailRegistrationAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_confirmemailregistrationasync"
label: ".ConfirmEmailRegistrationAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L114"
community: "0"
norm_label: ".confirmemailregistrationasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.ConfirmEmailRegistrationAsync()

- Category: `Services`
- Label: `.ConfirmEmailRegistrationAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L114`
- Graph Id: `authentications_authenticationservice_authenticationservice_confirmemailregistrationasync`
- Community: `0`

depends_on:: [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.UpdateUserAsync()]]
upstream:: [[AuthenticationService]]
downstream:: [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.UpdateUserAsync()]]

## Dependencies
- [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- [[AuthenticationService.UpdateUserAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- `calls` -> [[AuthenticationService.UpdateUserAsync()]]

## Upstream Relationships
### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

