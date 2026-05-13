---
title: "AuthenticationService.PhoneOtpLoginAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_phoneotploginasync"
label: ".PhoneOtpLoginAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L309"
community: "0"
norm_label: ".phoneotploginasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.PhoneOtpLoginAsync()

- Category: `Services`
- Label: `.PhoneOtpLoginAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L309`
- Graph Id: `authentications_authenticationservice_authenticationservice_phoneotploginasync`
- Community: `0`

depends_on:: [[AuthenticationService.BuildLoginResponse()]], [[AuthenticationService.CreateOrUpdateSessionAsync()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
upstream:: [[AuthenticationService]]
downstream:: [[AuthenticationService.BuildLoginResponse()]], [[AuthenticationService.CreateOrUpdateSessionAsync()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]

## Dependencies
- [[AuthenticationService.BuildLoginResponse()]]
- [[AuthenticationService.CreateOrUpdateSessionAsync()]]
- [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.BuildLoginResponse()]]
- `calls` -> [[AuthenticationService.CreateOrUpdateSessionAsync()]]
- `calls` -> [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]

## Upstream Relationships
### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

