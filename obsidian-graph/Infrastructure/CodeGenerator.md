---
title: "CodeGenerator"
type: "Infrastructure"
graph_id: "helpers_assert_codegenerator"
label: "CodeGenerator"
file_type: "code"
source_file: "Common/Utilities/Helpers/Assert.cs"
source_location: "L53"
community: "30"
norm_label: "codegenerator"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# CodeGenerator

- Category: `Infrastructure`
- Label: `CodeGenerator`
- Source: `Common/Utilities/Helpers/Assert.cs`
- Location: `L53`
- Graph Id: `helpers_assert_codegenerator`
- Community: `30`

depends_on:: [[Random]]
upstream:: [[Assert.cs]]
downstream:: [[CodeGenerator.CreateCodeVerifier()]], [[CodeGenerator.GenerateCodeChallenge()]], [[CodeGenerator.GenerateRandomNumber()]], [[CodeGenerator.GenerateRandomString()]], [[Random]]

## Dependencies
- [[Random]]

## Downstream Relationships
### Method
- `method` -> [[CodeGenerator.CreateCodeVerifier()]]
- `method` -> [[CodeGenerator.GenerateCodeChallenge()]]
- `method` -> [[CodeGenerator.GenerateRandomNumber()]]
- `method` -> [[CodeGenerator.GenerateRandomString()]]

### References
- `references` -> [[Random]]

## Upstream Relationships
### Contains
- [[Assert.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

