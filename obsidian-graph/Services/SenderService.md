---
title: "SenderService"
type: "Service"
graph_id: "notifiers_senderservice_senderservice"
label: "SenderService"
file_type: "code"
source_file: "Services/Services/Notifiers/SenderService.cs"
source_location: "L17"
community: "0"
norm_label: "senderservice"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# SenderService

- Category: `Services`
- Label: `SenderService`
- Source: `Services/Services/Notifiers/SenderService.cs`
- Location: `L17`
- Graph Id: `notifiers_senderservice_senderservice`
- Community: `0`

depends_on:: [[ILogger]], [[IScopedDependency (2)]], [[ISenderService (2)]], [[IWebHostEnvironment]], [[SiteSettings (2)]]
upstream:: [[SenderService.cs]]
downstream:: [[ILogger]], [[IScopedDependency (2)]], [[ISenderService (2)]], [[IWebHostEnvironment]], [[SenderService.BuildMailMessage()]], [[SenderService.SendEmailAsync()]], [[SenderService.SendEmailWithAttachmentAsync()]], [[SenderService.SendGeneralSmsAsync()]], [[SenderService.SendOtpSmsAsync()]], [[SenderService.SendTestSmsAsync()]], [[SiteSettings (2)]]

## Dependencies
- [[ILogger]]
- [[IScopedDependency (2)]]
- [[ISenderService (2)]]
- [[IWebHostEnvironment]]
- [[SiteSettings (2)]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IScopedDependency (2)]]
- `inherits` -> [[ISenderService (2)]]

### Method
- `method` -> [[SenderService.BuildMailMessage()]]
- `method` -> [[SenderService.SendEmailAsync()]]
- `method` -> [[SenderService.SendEmailWithAttachmentAsync()]]
- `method` -> [[SenderService.SendGeneralSmsAsync()]]
- `method` -> [[SenderService.SendOtpSmsAsync()]]
- `method` -> [[SenderService.SendTestSmsAsync()]]

### References
- `references` -> [[ILogger]]
- `references` -> [[IWebHostEnvironment]]
- `references` -> [[SiteSettings (2)]]

## Upstream Relationships
### Contains
- [[SenderService.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

