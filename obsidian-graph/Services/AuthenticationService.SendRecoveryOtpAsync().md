---
title: "AuthenticationService.SendRecoveryOtpAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_sendrecoveryotpasync"
label: ".SendRecoveryOtpAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L714"
community: "0"
norm_label: ".sendrecoveryotpasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.SendRecoveryOtpAsync()

- Category: `Services`
- Label: `.SendRecoveryOtpAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L714`
- Graph Id: `authentications_authenticationservice_authenticationservice_sendrecoveryotpasync`
- Community: `0`

depends_on:: [[AuthenticationService.SendActivationEmailAsync()]]
upstream:: [[AuthenticationService]], [[AuthenticationService.ForgotPasswordAsync()]]
downstream:: [[AuthenticationService.SendActivationEmailAsync()]]

## Dependencies
- [[AuthenticationService.SendActivationEmailAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.SendActivationEmailAsync()]]

## Upstream Relationships
### Calls
- [[AuthenticationService.ForgotPasswordAsync()]] -> `calls`

### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

