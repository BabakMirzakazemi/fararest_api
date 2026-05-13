---
title: "UserDataInitializer"
type: "Infrastructure"
graph_id: "datainitializer_userdatainitializer_userdatainitializer"
label: "UserDataInitializer"
file_type: "code"
source_file: "Services/DataInitializer/UserDataInitializer.cs"
source_location: "L5"
community: "21"
norm_label: "userdatainitializer"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# UserDataInitializer

- Category: `Infrastructure`
- Label: `UserDataInitializer`
- Source: `Services/DataInitializer/UserDataInitializer.cs`
- Location: `L5`
- Graph Id: `datainitializer_userdatainitializer_userdatainitializer`
- Community: `21`

depends_on:: [[IDataInitializer]], [[UserManager]]
upstream:: [[UserDataInitializer.cs]]
downstream:: [[IDataInitializer]], [[UserDataInitializer.InitializeData()]], [[UserManager]]

## Dependencies
- [[IDataInitializer]]
- [[UserManager]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IDataInitializer]]

### Method
- `method` -> [[UserDataInitializer.InitializeData()]]

### References
- `references` -> [[UserManager]]

## Upstream Relationships
### Contains
- [[UserDataInitializer.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

