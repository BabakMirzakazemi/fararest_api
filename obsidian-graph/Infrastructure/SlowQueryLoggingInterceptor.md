---
title: "SlowQueryLoggingInterceptor"
type: "Infrastructure"
graph_id: "performance_slowquerylogginginterceptor_slowquerylogginginterceptor"
label: "SlowQueryLoggingInterceptor"
file_type: "code"
source_file: "WebFramwork/Infrastructure/Performance/SlowQueryLoggingInterceptor.cs"
source_location: "L13"
community: "16"
norm_label: "slowquerylogginginterceptor"
graph_built_from_commit: "507549ea2d6c9403cff268bb380d1b1a85e37528"
---

# SlowQueryLoggingInterceptor

- Category: `Infrastructure`
- Label: `SlowQueryLoggingInterceptor`
- Source: `WebFramwork/Infrastructure/Performance/SlowQueryLoggingInterceptor.cs`
- Location: `L13`
- Graph Id: `performance_slowquerylogginginterceptor_slowquerylogginginterceptor`
- Community: `16`

depends_on:: [[ConcurrentDictionary]], [[DbCommandInterceptor]], [[QueryTuningSettings (2)]]
upstream:: [[SlowQueryLoggingInterceptor.cs]]
downstream:: [[ConcurrentDictionary]], [[DbCommandInterceptor]], [[QueryTuningSettings (2)]], [[SlowQueryLoggingInterceptor.CommandFailed()]], [[SlowQueryLoggingInterceptor.NonQueryExecuted()]], [[SlowQueryLoggingInterceptor.NonQueryExecuting()]], [[SlowQueryLoggingInterceptor.ReaderExecuted()]], [[SlowQueryLoggingInterceptor.ReaderExecuting()]], [[SlowQueryLoggingInterceptor.ScalarExecuted()]], [[SlowQueryLoggingInterceptor.ScalarExecuting()]], [[SlowQueryLoggingInterceptor.StartTiming()]], [[SlowQueryLoggingInterceptor.StopAndLogIfSlow()]]

## Dependencies
- [[ConcurrentDictionary]]
- [[DbCommandInterceptor]]
- [[QueryTuningSettings (2)]]

## Downstream Relationships
### Inherits
- `inherits` -> [[DbCommandInterceptor]]

### Method
- `method` -> [[SlowQueryLoggingInterceptor.CommandFailed()]]
- `method` -> [[SlowQueryLoggingInterceptor.NonQueryExecuted()]]
- `method` -> [[SlowQueryLoggingInterceptor.NonQueryExecuting()]]
- `method` -> [[SlowQueryLoggingInterceptor.ReaderExecuted()]]
- `method` -> [[SlowQueryLoggingInterceptor.ReaderExecuting()]]
- `method` -> [[SlowQueryLoggingInterceptor.ScalarExecuted()]]
- `method` -> [[SlowQueryLoggingInterceptor.ScalarExecuting()]]
- `method` -> [[SlowQueryLoggingInterceptor.StartTiming()]]
- `method` -> [[SlowQueryLoggingInterceptor.StopAndLogIfSlow()]]

### References
- `references` -> [[ConcurrentDictionary]]
- `references` -> [[QueryTuningSettings (2)]]

## Upstream Relationships
### Contains
- [[SlowQueryLoggingInterceptor.cs]] -> `contains`

## Note
Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.

