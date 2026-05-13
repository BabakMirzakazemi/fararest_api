---
title: "AuthenticationService.SendOtpAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_sendotpasync"
label: ".SendOtpAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L376"
community: "0"
norm_label: ".sendotpasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.SendOtpAsync()

- Category: `Services`
- Label: `.SendOtpAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L376`
- Graph Id: `authentications_authenticationservice_authenticationservice_sendotpasync`
- Community: `0`

depends_on:: [[AuthenticationService.GenerateOtp()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.UpdateUserAsync()]]
upstream:: [[AuthenticationService]]
downstream:: [[AuthenticationService.GenerateOtp()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.UpdateUserAsync()]]

## Dependencies
- [[AuthenticationService.GenerateOtp()]]
- [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- [[AuthenticationService.UpdateUserAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.GenerateOtp()]]
- `calls` -> [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- `calls` -> [[AuthenticationService.UpdateUserAsync()]]

## Upstream Relationships
### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

