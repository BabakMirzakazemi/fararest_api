---
title: "AuthenticationService.ResetPasswordWithOtpAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_resetpasswordwithotpasync"
label: ".ResetPasswordWithOtpAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L414"
community: "0"
norm_label: ".resetpasswordwithotpasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.ResetPasswordWithOtpAsync()

- Category: `Services`
- Label: `.ResetPasswordWithOtpAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L414`
- Graph Id: `authentications_authenticationservice_authenticationservice_resetpasswordwithotpasync`
- Community: `0`

depends_on:: [[AuthenticationService.FindUserByIdentifierAsync()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.RevokeAllUserSessionsAsync()]], [[AuthenticationService.UpdateUserAsync()]]
upstream:: [[AuthenticationService]]
downstream:: [[AuthenticationService.FindUserByIdentifierAsync()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.RevokeAllUserSessionsAsync()]], [[AuthenticationService.UpdateUserAsync()]]

## Dependencies
- [[AuthenticationService.FindUserByIdentifierAsync()]]
- [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- [[AuthenticationService.RevokeAllUserSessionsAsync()]]
- [[AuthenticationService.UpdateUserAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.FindUserByIdentifierAsync()]]
- `calls` -> [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- `calls` -> [[AuthenticationService.RevokeAllUserSessionsAsync()]]
- `calls` -> [[AuthenticationService.UpdateUserAsync()]]

## Upstream Relationships
### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

