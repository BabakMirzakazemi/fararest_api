---
title: "ServiceCollectionExtensions.RegisterOpenTelemetry()"
type: "Infrastructure"
graph_id: "configuration_servicecollectionextensions_servicecollectionextensions_registeropentelemetry"
label: ".RegisterOpenTelemetry()"
file_type: "code"
source_file: "WebFramwork/Configuration/ServiceCollectionExtensions.cs"
source_location: "L111"
community: "22"
norm_label: ".registeropentelemetry()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# ServiceCollectionExtensions.RegisterOpenTelemetry()

- Category: `Infrastructure`
- Label: `.RegisterOpenTelemetry()`
- Source: `WebFramwork/Configuration/ServiceCollectionExtensions.cs`
- Location: `L111`
- Graph Id: `configuration_servicecollectionextensions_servicecollectionextensions_registeropentelemetry`
- Community: `22`

depends_on:: [[ServiceCollectionExtensions.AddOtlpExporterIfConfigured()]], [[ServiceCollectionExtensions.IsExcludedTelemetryPath()]]
upstream:: [[ServiceCollectionExtensions]], [[ServiceCollectionExtensions.AddPerformanceInfrastructure()]]
downstream:: [[ServiceCollectionExtensions.AddOtlpExporterIfConfigured()]], [[ServiceCollectionExtensions.IsExcludedTelemetryPath()]]

## Dependencies
- [[ServiceCollectionExtensions.AddOtlpExporterIfConfigured()]]
- [[ServiceCollectionExtensions.IsExcludedTelemetryPath()]]

## Downstream Relationships
### Calls
- `calls` -> [[ServiceCollectionExtensions.AddOtlpExporterIfConfigured()]]
- `calls` -> [[ServiceCollectionExtensions.IsExcludedTelemetryPath()]]

## Upstream Relationships
### Calls
- [[ServiceCollectionExtensions.AddPerformanceInfrastructure()]] -> `calls`

### Method
- [[ServiceCollectionExtensions]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

