---
title: "AuthenticationService.ForgotPasswordAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_forgotpasswordasync"
label: ".ForgotPasswordAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L398"
community: "0"
norm_label: ".forgotpasswordasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.ForgotPasswordAsync()

- Category: `Services`
- Label: `.ForgotPasswordAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L398`
- Graph Id: `authentications_authenticationservice_authenticationservice_forgotpasswordasync`
- Community: `0`

depends_on:: [[AuthenticationService.FindUserByIdentifierAsync()]], [[AuthenticationService.GenerateOtp()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.SendRecoveryOtpAsync()]], [[AuthenticationService.UpdateUserAsync()]]
upstream:: [[AuthenticationService]]
downstream:: [[AuthenticationService.FindUserByIdentifierAsync()]], [[AuthenticationService.GenerateOtp()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.SendRecoveryOtpAsync()]], [[AuthenticationService.UpdateUserAsync()]]

## Dependencies
- [[AuthenticationService.FindUserByIdentifierAsync()]]
- [[AuthenticationService.GenerateOtp()]]
- [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- [[AuthenticationService.SendRecoveryOtpAsync()]]
- [[AuthenticationService.UpdateUserAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.FindUserByIdentifierAsync()]]
- `calls` -> [[AuthenticationService.GenerateOtp()]]
- `calls` -> [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- `calls` -> [[AuthenticationService.SendRecoveryOtpAsync()]]
- `calls` -> [[AuthenticationService.UpdateUserAsync()]]

## Upstream Relationships
### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

