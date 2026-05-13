---
title: "AuthenticationService"
type: "Service"
graph_id: "authentications_authenticationservice_authenticationservice"
label: "AuthenticationService"
file_type: "code"
source_file: "Services/Services/Authentications/AuthenticationService.cs"
source_location: "L30"
community: "0"
norm_label: "authenticationservice"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AuthenticationService

- Category: `Services`
- Label: `AuthenticationService`
- Source: `Services/Services/Authentications/AuthenticationService.cs`
- Location: `L30`
- Graph Id: `authentications_authenticationservice_authenticationservice`
- Community: `0`

depends_on:: [[IAuthenticationService (2)]], [[IEffectiveAuthorizationService (2)]], [[IHttpContextAccessor]], [[IJwtService (2)]], [[int]], [[IRepository]], [[IScopedDependency (2)]], [[ISenderService (2)]], [[IUserContext (2)]], [[IWebHostEnvironment]], [[SignInManager]], [[SiteSettings (2)]], [[string]], [[UserManager]]
upstream:: [[AuthenticationService.cs]]
downstream:: [[AuthenticationService.AddDefaultRoleToUserAsync()]], [[AuthenticationService.BuildEmailActivationLink()]], [[AuthenticationService.BuildLoginResponse()]], [[AuthenticationService.CompletePhoneRegistrationAsync()]], [[AuthenticationService.ComputeSha256()]], [[AuthenticationService.ConfirmEmailRegistrationAsync()]], [[AuthenticationService.CreateOrUpdateSessionAsync()]], [[AuthenticationService.CreateUserAsync()]], [[AuthenticationService.EmailPasswordLoginAsync()]], [[AuthenticationService.EnsureMfaFeatureAsync()]], [[AuthenticationService.FindUserByIdentifierAsync()]], [[AuthenticationService.ForgotPasswordAsync()]], [[AuthenticationService.GenerateOtp()]], [[AuthenticationService.GetCurrentBearerHash()]], [[AuthenticationService.GetMfaStatusAsync()]], [[AuthenticationService.GetMyAuthorizationAsync()]], [[AuthenticationService.GetMySessionsAsync()]], [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]], [[AuthenticationService.HasRecentActivationEmail()]], [[AuthenticationService.HasRecentPhoneRegistrationOtp()]], [[AuthenticationService.MapUserIdToLegacyInt()]], [[AuthenticationService.OtpLoginAsync()]], [[AuthenticationService.ParseDeviceType()]], [[AuthenticationService.PasswordLoginAsync()]], [[AuthenticationService.PhoneOtpLoginAsync()]], [[AuthenticationService.RegisterWithEmailAsync()]], [[AuthenticationService.ResendEmailActivationLinkAsync()]], [[AuthenticationService.ResendPhoneRegistrationOtpAsync()]], [[AuthenticationService.ResetPasswordWithOtpAsync()]], [[AuthenticationService.RevokeAllUserSessionsAsync()]], [[AuthenticationService.RevokeOtherSessionsAsync()]], [[AuthenticationService.RevokeSessionAsync()]], [[AuthenticationService.SendActivationEmailAsync()]], [[AuthenticationService.SendOtpAsync()]], [[AuthenticationService.SendPhoneLoginOtpAsync()]], [[AuthenticationService.SendRecoveryOtpAsync()]], [[AuthenticationService.SetMfaStatusAsync()]], [[AuthenticationService.SignOutAsync()]], [[AuthenticationService.StartPhoneRegistrationAsync()]], [[AuthenticationService.UpdatePasswordAsync()]], [[AuthenticationService.UpdateUserAsync()]], [[IAuthenticationService (2)]], [[IEffectiveAuthorizationService (2)]], [[IHttpContextAccessor]], [[IJwtService (2)]], [[int]], [[IRepository]], [[IScopedDependency (2)]], [[ISenderService (2)]], [[IUserContext (2)]], [[IWebHostEnvironment]], [[SignInManager]], [[SiteSettings (2)]], [[string]], [[UserManager]]

## Dependencies
- [[IAuthenticationService (2)]]
- [[IEffectiveAuthorizationService (2)]]
- [[IHttpContextAccessor]]
- [[IJwtService (2)]]
- [[int]]
- [[IRepository]]
- [[IScopedDependency (2)]]
- [[ISenderService (2)]]
- [[IUserContext (2)]]
- [[IWebHostEnvironment]]
- [[SignInManager]]
- [[SiteSettings (2)]]
- [[string]]
- [[UserManager]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IAuthenticationService (2)]]
- `inherits` -> [[IScopedDependency (2)]]

### Method
- `method` -> [[AuthenticationService.AddDefaultRoleToUserAsync()]]
- `method` -> [[AuthenticationService.BuildEmailActivationLink()]]
- `method` -> [[AuthenticationService.BuildLoginResponse()]]
- `method` -> [[AuthenticationService.CompletePhoneRegistrationAsync()]]
- `method` -> [[AuthenticationService.ComputeSha256()]]
- `method` -> [[AuthenticationService.ConfirmEmailRegistrationAsync()]]
- `method` -> [[AuthenticationService.CreateOrUpdateSessionAsync()]]
- `method` -> [[AuthenticationService.CreateUserAsync()]]
- `method` -> [[AuthenticationService.EmailPasswordLoginAsync()]]
- `method` -> [[AuthenticationService.EnsureMfaFeatureAsync()]]
- `method` -> [[AuthenticationService.FindUserByIdentifierAsync()]]
- `method` -> [[AuthenticationService.ForgotPasswordAsync()]]
- `method` -> [[AuthenticationService.GenerateOtp()]]
- `method` -> [[AuthenticationService.GetCurrentBearerHash()]]
- `method` -> [[AuthenticationService.GetMfaStatusAsync()]]
- `method` -> [[AuthenticationService.GetMyAuthorizationAsync()]]
- `method` -> [[AuthenticationService.GetMySessionsAsync()]]
- `method` -> [[AuthenticationService.GetOrCreateConfirmationCodeAsync()]]
- `method` -> [[AuthenticationService.HasRecentActivationEmail()]]
- `method` -> [[AuthenticationService.HasRecentPhoneRegistrationOtp()]]
- `method` -> [[AuthenticationService.MapUserIdToLegacyInt()]]
- `method` -> [[AuthenticationService.OtpLoginAsync()]]
- `method` -> [[AuthenticationService.ParseDeviceType()]]
- `method` -> [[AuthenticationService.PasswordLoginAsync()]]
- `method` -> [[AuthenticationService.PhoneOtpLoginAsync()]]
- `method` -> [[AuthenticationService.RegisterWithEmailAsync()]]
- `method` -> [[AuthenticationService.ResendEmailActivationLinkAsync()]]
- `method` -> [[AuthenticationService.ResendPhoneRegistrationOtpAsync()]]
- `method` -> [[AuthenticationService.ResetPasswordWithOtpAsync()]]
- `method` -> [[AuthenticationService.RevokeAllUserSessionsAsync()]]
- `method` -> [[AuthenticationService.RevokeOtherSessionsAsync()]]
- `method` -> [[AuthenticationService.RevokeSessionAsync()]]
- `method` -> [[AuthenticationService.SendActivationEmailAsync()]]
- `method` -> [[AuthenticationService.SendOtpAsync()]]
- `method` -> [[AuthenticationService.SendPhoneLoginOtpAsync()]]
- `method` -> [[AuthenticationService.SendRecoveryOtpAsync()]]
- `method` -> [[AuthenticationService.SetMfaStatusAsync()]]
- `method` -> [[AuthenticationService.SignOutAsync()]]
- `method` -> [[AuthenticationService.StartPhoneRegistrationAsync()]]
- `method` -> [[AuthenticationService.UpdatePasswordAsync()]]
- `method` -> [[AuthenticationService.UpdateUserAsync()]]

### References
- `references` -> [[IEffectiveAuthorizationService (2)]]
- `references` -> [[IHttpContextAccessor]]
- `references` -> [[IJwtService (2)]]
- `references` -> [[int]]
- `references` -> [[IRepository]]
- `references` -> [[ISenderService (2)]]
- `references` -> [[IUserContext (2)]]
- `references` -> [[IWebHostEnvironment]]
- `references` -> [[SignInManager]]
- `references` -> [[SiteSettings (2)]]
- `references` -> [[string]]
- `references` -> [[UserManager]]

## Upstream Relationships
### Contains
- [[AuthenticationService.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

