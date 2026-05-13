---
title: "ServiceCollectionExtensions.AddOtlpExporterIfConfigured()"
type: "Infrastructure"
graph_id: "configuration_servicecollectionextensions_servicecollectionextensions_addotlpexporterifconfigured"
label: ".AddOtlpExporterIfConfigured()"
file_type: "code"
source_file: "WebFramwork/Configuration/ServiceCollectionExtensions.cs"
source_location: "L164"
community: "22"
norm_label: ".addotlpexporterifconfigured()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# ServiceCollectionExtensions.AddOtlpExporterIfConfigured()

- Category: `Infrastructure`
- Label: `.AddOtlpExporterIfConfigured()`
- Source: `WebFramwork/Configuration/ServiceCollectionExtensions.cs`
- Location: `L164`
- Graph Id: `configuration_servicecollectionextensions_servicecollectionextensions_addotlpexporterifconfigured`
- Community: `22`

depends_on:: [[ServiceCollectionExtensions.ParseOtlpProtocol()]]
upstream:: [[ServiceCollectionExtensions]], [[ServiceCollectionExtensions.RegisterOpenTelemetry()]]
downstream:: [[ServiceCollectionExtensions.ParseOtlpProtocol()]]

## Dependencies
- [[ServiceCollectionExtensions.ParseOtlpProtocol()]]

## Downstream Relationships
### Calls
- `calls` -> [[ServiceCollectionExtensions.ParseOtlpProtocol()]]

## Upstream Relationships
### Calls
- [[ServiceCollectionExtensions.RegisterOpenTelemetry()]] -> `calls`

### Method
- [[ServiceCollectionExtensions]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

