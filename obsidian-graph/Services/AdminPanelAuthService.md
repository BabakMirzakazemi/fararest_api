---
title: "AdminPanelAuthService"
type: "Service"
graph_id: "infrastructure_adminpanelauthservice_adminpanelauthservice"
label: "AdminPanelAuthService"
file_type: "code"
source_file: "AdminPanel/Infrastructure/AdminPanelAuthService.cs"
source_location: "L17"
community: "41"
norm_label: "adminpanelauthservice"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AdminPanelAuthService

- Category: `Services`
- Label: `AdminPanelAuthService`
- Source: `AdminPanel/Infrastructure/AdminPanelAuthService.cs`
- Location: `L17`
- Graph Id: `infrastructure_adminpanelauthservice_adminpanelauthservice`
- Community: `41`

depends_on:: [[AdminPanelAuthenticationSettings]], [[IAdminPanelAuthService]]
upstream:: [[AdminPanelAuthService.cs]]
downstream:: [[AdminPanelAuthenticationSettings]], [[AdminPanelAuthService.HasAdminAccess()]], [[AdminPanelAuthService.SignInWithEmailPasswordAsync()]], [[AdminPanelAuthService.SignOutAsync()]], [[IAdminPanelAuthService]]

## Dependencies
- [[AdminPanelAuthenticationSettings]]
- [[IAdminPanelAuthService]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IAdminPanelAuthService]]

### Method
- `method` -> [[AdminPanelAuthService.HasAdminAccess()]]
- `method` -> [[AdminPanelAuthService.SignInWithEmailPasswordAsync()]]
- `method` -> [[AdminPanelAuthService.SignOutAsync()]]

### References
- `references` -> [[AdminPanelAuthenticationSettings]]

## Upstream Relationships
### Contains
- [[AdminPanelAuthService.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

