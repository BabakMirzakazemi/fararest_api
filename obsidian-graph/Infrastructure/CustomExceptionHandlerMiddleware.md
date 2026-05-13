---
title: "CustomExceptionHandlerMiddleware"
type: "Infrastructure"
graph_id: "middlewares_customexceptionhandlermiddleware_customexceptionhandlermiddleware"
label: "CustomExceptionHandlerMiddleware"
file_type: "code"
source_file: "WebFramwork/Middlewares/CustomExceptionHandlerMiddleware.cs"
source_location: "L13"
community: "0"
norm_label: "customexceptionhandlermiddleware"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# CustomExceptionHandlerMiddleware

- Category: `Infrastructure`
- Label: `CustomExceptionHandlerMiddleware`
- Source: `WebFramwork/Middlewares/CustomExceptionHandlerMiddleware.cs`
- Location: `L13`
- Graph Id: `middlewares_customexceptionhandlermiddleware_customexceptionhandlermiddleware`
- Community: `0`

depends_on:: [[ILogger]], [[IWebHostEnvironment]], [[RequestDelegate]]
upstream:: [[CustomExceptionHandlerMiddleware.cs]]
downstream:: [[CustomExceptionHandlerMiddleware.Invoke()]], [[ILogger]], [[IWebHostEnvironment]], [[RequestDelegate]]

## Dependencies
- [[ILogger]]
- [[IWebHostEnvironment]]
- [[RequestDelegate]]

## Downstream Relationships
### Method
- `method` -> [[CustomExceptionHandlerMiddleware.Invoke()]]

### References
- `references` -> [[ILogger]]
- `references` -> [[IWebHostEnvironment]]
- `references` -> [[RequestDelegate]]

## Upstream Relationships
### Contains
- [[CustomExceptionHandlerMiddleware.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

