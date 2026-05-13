---
title: "AuthenticationService.RegisterWithEmailAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_registerwithemailasync"
label: ".RegisterWithEmailAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L80"
community: "0"
norm_label: ".registerwithemailasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.RegisterWithEmailAsync()

- Category: `Services`
- Label: `.RegisterWithEmailAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L80`
- Graph Id: `authentications_authenticationservice_authenticationservice_registerwithemailasync`
- Community: `0`

depends_on:: [[AuthenticationService.AddDefaultRoleToUserAsync()]], [[AuthenticationService.CreateUserAsync()]], [[AuthenticationService.GenerateOtp()]], [[AuthenticationService.SendActivationEmailAsync()]]
upstream:: [[AuthenticationService]]
downstream:: [[AuthenticationService.AddDefaultRoleToUserAsync()]], [[AuthenticationService.CreateUserAsync()]], [[AuthenticationService.GenerateOtp()]], [[AuthenticationService.SendActivationEmailAsync()]]

## Dependencies
- [[AuthenticationService.AddDefaultRoleToUserAsync()]]
- [[AuthenticationService.CreateUserAsync()]]
- [[AuthenticationService.GenerateOtp()]]
- [[AuthenticationService.SendActivationEmailAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.AddDefaultRoleToUserAsync()]]
- `calls` -> [[AuthenticationService.CreateUserAsync()]]
- `calls` -> [[AuthenticationService.GenerateOtp()]]
- `calls` -> [[AuthenticationService.SendActivationEmailAsync()]]

## Upstream Relationships
### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

