---
title: "AssemblyHelper.FindMyAssemblies()"
type: "Infrastructure"
graph_id: "helpers_assemblyhelper_assemblyhelper_findmyassemblies"
label: ".FindMyAssemblies()"
file_type: "code"
source_file: "Common/Utilities/Helpers/AssemblyHelper.cs"
source_location: "L48"
community: "15"
norm_label: ".findmyassemblies()"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# AssemblyHelper.FindMyAssemblies()

- Category: `Infrastructure`
- Label: `.FindMyAssemblies()`
- Source: `Common/Utilities/Helpers/AssemblyHelper.cs`
- Location: `L48`
- Graph Id: `helpers_assemblyhelper_assemblyhelper_findmyassemblies`
- Community: `15`

depends_on:: [[AssemblyHelper.LoadAllBinDirectoryAssemblies()]]
upstream:: [[AssemblyHelper]], [[AssemblyHelper.FindAllTypes()]]
downstream:: [[AssemblyHelper.LoadAllBinDirectoryAssemblies()]]

## Dependencies
- [[AssemblyHelper.LoadAllBinDirectoryAssemblies()]]

## Downstream Relationships
### Calls
- `calls` -> [[AssemblyHelper.LoadAllBinDirectoryAssemblies()]]

## Upstream Relationships
### Calls
- [[AssemblyHelper.FindAllTypes()]] -> `calls`

### Method
- [[AssemblyHelper]] -> `method`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

