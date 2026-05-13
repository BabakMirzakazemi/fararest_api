---
title: "AuthenticationService.ResendEmailActivationLinkAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_resendemailactivationlinkasync"
label: ".ResendEmailActivationLinkAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L149"
community: "0"
norm_label: ".resendemailactivationlinkasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.ResendEmailActivationLinkAsync()

- Category: `Services`
- Label: `.ResendEmailActivationLinkAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L149`
- Graph Id: `authentications_authenticationservice_authenticationservice_resendemailactivationlinkasync`
- Community: `0`

depends_on:: [[AuthenticationService.GenerateOtp()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.HasRecentActivationEmail()]], [[AuthenticationService.SendActivationEmailAsync()]], [[AuthenticationService.UpdateUserAsync()]]
upstream:: [[AuthenticationService]]
downstream:: [[AuthenticationService.GenerateOtp()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.HasRecentActivationEmail()]], [[AuthenticationService.SendActivationEmailAsync()]], [[AuthenticationService.UpdateUserAsync()]]

## Dependencies
- [[AuthenticationService.GenerateOtp()]]
- [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- [[AuthenticationService.HasRecentActivationEmail()]]
- [[AuthenticationService.SendActivationEmailAsync()]]
- [[AuthenticationService.UpdateUserAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.GenerateOtp()]]
- `calls` -> [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- `calls` -> [[AuthenticationService.HasRecentActivationEmail()]]
- `calls` -> [[AuthenticationService.SendActivationEmailAsync()]]
- `calls` -> [[AuthenticationService.UpdateUserAsync()]]

## Upstream Relationships
### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

