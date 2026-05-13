---
title: "ServiceRepository"
type: "Service"
graph_id: "repositories_servicerepository_servicerepository"
label: "ServiceRepository"
file_type: "code"
source_file: "Services/Services/Repositories/ServiceRepository.cs"
source_location: "L13"
community: "1"
norm_label: "servicerepository"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# ServiceRepository

- Category: `Services`
- Label: `ServiceRepository`
- Source: `Services/Services/Repositories/ServiceRepository.cs`
- Location: `L13`
- Graph Id: `repositories_servicerepository_servicerepository`
- Community: `1`

depends_on:: [[IMapper]], [[IRepository]], [[IScopedDependency (2)]], [[IServiceRepository (2)]]
upstream:: [[ServiceRepository.cs]]
downstream:: [[IMapper]], [[IRepository]], [[IScopedDependency (2)]], [[IServiceRepository (2)]], [[ServiceRepository.Create()]], [[ServiceRepository.Delete()]], [[ServiceRepository.Get()]], [[ServiceRepository.MapToSelectDto()]], [[ServiceRepository.RequireKey()]], [[ServiceRepository.Select()]], [[ServiceRepository.SelectByCursor()]], [[ServiceRepository.Update()]], [[ServiceRepository.UpdateCustomProperties()]], [[ServiceRepository.UpdateProperty()]]

## Dependencies
- [[IMapper]]
- [[IRepository]]
- [[IScopedDependency (2)]]
- [[IServiceRepository (2)]]

## Downstream Relationships
### Inherits
- `inherits` -> [[IScopedDependency (2)]]
- `inherits` -> [[IServiceRepository (2)]]

### Method
- `method` -> [[ServiceRepository.Create()]]
- `method` -> [[ServiceRepository.Delete()]]
- `method` -> [[ServiceRepository.Get()]]
- `method` -> [[ServiceRepository.MapToSelectDto()]]
- `method` -> [[ServiceRepository.RequireKey()]]
- `method` -> [[ServiceRepository.Select()]]
- `method` -> [[ServiceRepository.SelectByCursor()]]
- `method` -> [[ServiceRepository.Update()]]
- `method` -> [[ServiceRepository.UpdateCustomProperties()]]
- `method` -> [[ServiceRepository.UpdateProperty()]]

### References
- `references` -> [[IMapper]]
- `references` -> [[IRepository]]

## Upstream Relationships
### Contains
- [[ServiceRepository.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

