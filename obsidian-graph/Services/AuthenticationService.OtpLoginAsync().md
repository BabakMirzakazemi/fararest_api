---
title: "AuthenticationService.OtpLoginAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_otploginasync"
label: ".OtpLoginAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L354"
community: "0"
norm_label: ".otploginasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.OtpLoginAsync()

- Category: `Services`
- Label: `.OtpLoginAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L354`
- Graph Id: `authentications_authenticationservice_authenticationservice_otploginasync`
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

