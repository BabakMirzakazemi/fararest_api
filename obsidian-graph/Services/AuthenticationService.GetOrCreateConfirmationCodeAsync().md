---
title: "AuthenticationService.GetOrCreateConfirmationCodeAsync()"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice_getorcreateconfirmationcodeasync"
label: ".GetOrCreateConfirmationCodeAsync()"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L618"
community: "0"
norm_label: ".getorcreateconfirmationcodeasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService.GetOrCreateConfirmationCodeAsync()

- Category: `Services`
- Label: `.GetOrCreateConfirmationCodeAsync()`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L618`
- Graph Id: `authentications_authenticationservice_authenticationservice_getorcreateconfirmationcodeasync`
- Community: `0`

depends_on:: [[AuthenticationService.UpdateUserAsync()]]
upstream:: [[AuthenticationService]], [[AuthenticationService.CompletePhoneRegistrationAsync()]], [[AuthenticationService.ConfirmEmailRegistrationAsync()]], [[AuthenticationService.ForgotPasswordAsync()]], [[AuthenticationService.OtpLoginAsync()]], [[AuthenticationService.PhoneOtpLoginAsync()]], [[AuthenticationService.ResendEmailActivationLinkAsync()]], [[AuthenticationService.ResendPhoneRegistrationOtpAsync()]], [[AuthenticationService.ResetPasswordWithOtpAsync()]], [[AuthenticationService.SendOtpAsync()]], [[AuthenticationService.SendPhoneLoginOtpAsync()]], [[AuthenticationService.SetMfaStatusAsync()]], [[AuthenticationService.UpdatePasswordAsync()]]
downstream:: [[AuthenticationService.UpdateUserAsync()]]

## Dependencies
- [[AuthenticationService.UpdateUserAsync()]]

## Downstream Relationships
### Calls
- `calls` -> [[AuthenticationService.UpdateUserAsync()]]

## Upstream Relationships
### Calls
- [[AuthenticationService.CompletePhoneRegistrationAsync()]] -> `calls`
- [[AuthenticationService.ConfirmEmailRegistrationAsync()]] -> `calls`
- [[AuthenticationService.ForgotPasswordAsync()]] -> `calls`
- [[AuthenticationService.OtpLoginAsync()]] -> `calls`
- [[AuthenticationService.PhoneOtpLoginAsync()]] -> `calls`
- [[AuthenticationService.ResendEmailActivationLinkAsync()]] -> `calls`
- [[AuthenticationService.ResendPhoneRegistrationOtpAsync()]] -> `calls`
- [[AuthenticationService.ResetPasswordWithOtpAsync()]] -> `calls`
- [[AuthenticationService.SendOtpAsync()]] -> `calls`
- [[AuthenticationService.SendPhoneLoginOtpAsync()]] -> `calls`
- [[AuthenticationService.SetMfaStatusAsync()]] -> `calls`
- [[AuthenticationService.UpdatePasswordAsync()]] -> `calls`

### Method
- [[AuthenticationService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

