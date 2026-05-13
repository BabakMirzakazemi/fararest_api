---
title: "SenderService.SendEmailAsync()"
type: "Service"
graph_id: "notifiers_senderservice_senderservice_sendemailasync"
label: ".SendEmailAsync()"
file_type: "code"
source_file: "Services/Services/Notifiers/SenderService.cs"
source_location: "L67"
community: "0"
norm_label: ".sendemailasync()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# SenderService.SendEmailAsync()

- Category: `Services`
- Label: `.SendEmailAsync()`
- Source: `Services/Services/Notifiers/SenderService.cs`
- Location: `L67`
- Graph Id: `notifiers_senderservice_senderservice_sendemailasync`
- Community: `0`

depends_on:: [[SenderService.BuildMailMessage()]]
upstream:: [[SenderService]], [[SenderService.SendEmailWithAttachmentAsync()]]
downstream:: [[SenderService.BuildMailMessage()]]

## Dependencies
- [[SenderService.BuildMailMessage()]]

## Downstream Relationships
### Calls
- `calls` -> [[SenderService.BuildMailMessage()]]

## Upstream Relationships
### Calls
- [[SenderService.SendEmailWithAttachmentAsync()]] -> `calls`

### Method
- [[SenderService]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

