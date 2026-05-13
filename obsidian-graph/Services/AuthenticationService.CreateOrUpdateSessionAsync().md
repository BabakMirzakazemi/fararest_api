---
title: "AuthenticationService.CreateOrUpdateSessionAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_createorupdatesessionasync"
label: ".CreateOrUpdateSessionAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L727"
community: "0"
norm_label: ".createorupdatesessionasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.CreateOrUpdateSessionAsync()

- Category: `Services`
- Label: `.CreateOrUpdateSessionAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L727`
- Graph Id: `authentications_authenticationservice_authenticationservice_createorupdatesessionasync`
- Community: `0`

depends_on:: [[AuthenticationService.ComputeSha256()]], [[AuthenticationService.MapUserIdToLegacyInt()]], [[AuthenticationService.ParseDeviceType()]]
upstream:: [[AuthenticationService]], [[AuthenticationService.EmailPasswordLoginAsync()]], [[AuthenticationService.OtpLoginAsync()]], [[AuthenticationService.PasswordLoginAsync()]], [[AuthenticationService.PhoneOtpLoginAsync()]]
downstream:: [[AuthenticationService.ComputeSha256()]], [[AuthenticationService.MapUserIdToLegacyInt()]], [[AuthenticationService.ParseDeviceType()]]

## Dependencies
- [[AuthenticationService.ComputeSha256()]]
- [[AuthenticationService.MapUserIdToLegacyInt()]]
- [[AuthenticationService.ParseDeviceType()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.ComputeSha256()]]
- `calls` -> [[AuthenticationService.MapUserIdToLegacyInt()]]
- `calls` -> [[AuthenticationService.ParseDeviceType()]]

## Upstream Relationships
### Calls
- [[AuthenticationService.EmailPasswordLoginAsync()]] -> `calls`
- [[AuthenticationService.OtpLoginAsync()]] -> `calls`
- [[AuthenticationService.PasswordLoginAsync()]] -> `calls`
- [[AuthenticationService.PhoneOtpLoginAsync()]] -> `calls`

### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

