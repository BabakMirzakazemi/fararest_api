---
title: "ApplicationDbContext (2)"
type: "Infrastructure"
graph_id: "data_applicationdbcontext_applicationdbcontext"
label: "ApplicationDbContext"
file_type: "code"
source_file: "Data/ApplicationDbContext.cs"
source_location: "L11"
community: "48"
norm_label: "applicationdbcontext"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# ApplicationDbContext (2)

- Category: `Infrastructure`
- Label: `ApplicationDbContext`
- Source: `Data/ApplicationDbContext.cs`
- Location: `L11`
- Graph Id: `data_applicationdbcontext_applicationdbcontext`
- Community: `48`

depends_on:: [[IdentityDbContext]]
upstream:: [[ApplicationDbContext.cs]]
downstream:: [[ApplicationDbContext._cleanString()]], [[ApplicationDbContext.OnModelCreating()]], [[ApplicationDbContext.SaveChanges()]], [[ApplicationDbContext.SaveChangesAsync()]], [[IdentityDbContext]]

## Dependencies
- [[IdentityDbContext]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IdentityDbContext]]

### Method
- `method` -> [[ApplicationDbContext._cleanString()]]
- `method` -> [[ApplicationDbContext.OnModelCreating()]]
- `method` -> [[ApplicationDbContext.SaveChanges()]]
- `method` -> [[ApplicationDbContext.SaveChangesAsync()]]

## Upstream Relationships
### Contains
- [[ApplicationDbContext.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

