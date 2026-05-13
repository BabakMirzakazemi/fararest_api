---
title: "PasswordValidator"
type: "Infrastructure"
graph_id: "validators_passwordvalidator_passwordvalidator"
label: "PasswordValidator"
file_type: "code"
source_file: "Services/DTOs/Common/Validators/PasswordValidator.cs"
source_location: "L7"
community: "15"
norm_label: "passwordvalidator"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# PasswordValidator

- Category: `Infrastructure`
- Label: `PasswordValidator`
- Source: `Services/DTOs/Common/Validators/PasswordValidator.cs`
- Location: `L7`
- Graph Id: `validators_passwordvalidator_passwordvalidator`
- Community: `15`

depends_on:: [[Func]], [[PropertyValidator]]
upstream:: [[PasswordValidator.cs]]
downstream:: [[Func]], [[PasswordValidator.GetDefaultMessageTemplate()]], [[PasswordValidator.IsValid()]], [[PropertyValidator]]

## Dependencies
- [[Func]]
- [[PropertyValidator]]

## Downstream Relationships
### Inherits
- `inherits` -> [[PropertyValidator]]

### Method
- `method` -> [[PasswordValidator.GetDefaultMessageTemplate()]]
- `method` -> [[PasswordValidator.IsValid()]]

### References
- `references` -> [[Func]]

## Upstream Relationships
### Contains
- [[PasswordValidator.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

