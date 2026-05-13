---
title: "AuthenticationService.SendPhoneLoginOtpAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_sendphoneloginotpasync"
label: ".SendPhoneLoginOtpAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L283"
community: "0"
norm_label: ".sendphoneloginotpasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.SendPhoneLoginOtpAsync()

- Category: `Services`
- Label: `.SendPhoneLoginOtpAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L283`
- Graph Id: `authentications_authenticationservice_authenticationservice_sendphoneloginotpasync`
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

