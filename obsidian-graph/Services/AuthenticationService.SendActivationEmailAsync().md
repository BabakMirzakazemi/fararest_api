---
title: "AuthenticationService.SendActivationEmailAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_sendactivationemailasync"
label: ".SendActivationEmailAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L661"
community: "0"
norm_label: ".sendactivationemailasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.SendActivationEmailAsync()

- Category: `Services`
- Label: `.SendActivationEmailAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L661`
- Graph Id: `authentications_authenticationservice_authenticationservice_sendactivationemailasync`
- Community: `0`

depends_on:: [[AuthenticationService.BuildEmailActivationLink()]]
upstream:: [[AuthenticationService]], [[AuthenticationService.RegisterWithEmailAsync()]], [[AuthenticationService.ResendEmailActivationLinkAsync()]], [[AuthenticationService.SendRecoveryOtpAsync()]]
downstream:: [[AuthenticationService.BuildEmailActivationLink()]]

## Dependencies
- [[AuthenticationService.BuildEmailActivationLink()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.BuildEmailActivationLink()]]

## Upstream Relationships
### Calls
- [[AuthenticationService.RegisterWithEmailAsync()]] -> `calls`
- [[AuthenticationService.ResendEmailActivationLinkAsync()]] -> `calls`
- [[AuthenticationService.SendRecoveryOtpAsync()]] -> `calls`

### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

