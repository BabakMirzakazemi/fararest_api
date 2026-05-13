param(
    [string]$InputPath = "graphify-out/graph.json",
    [string]$OutputPath = "obsidian-graph",
    [switch]$Clean
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$resolvedInput = Join-Path $root $InputPath
$resolvedOutput = Join-Path $root $OutputPath
$folders = @("Controllers", "Services", "Entities", "Repositories", "Infrastructure")
$folderTypes = @{
    Controllers    = "Controller"
    Services       = "Service"
    Entities       = "Entity"
    Repositories   = "Repository"
    Infrastructure = "Infrastructure"
}
$dependencyRelations = @("calls", "references", "inherits", "imports")
$downstreamRelations = @("contains", "method", "calls", "references", "inherits", "imports")
$upstreamRelations = @("contains", "method", "calls", "references", "inherits", "imports")

function Normalize-PathValue {
    param([AllowNull()][string]$Value)
    if ([string]::IsNullOrWhiteSpace($Value)) {
        return ""
    }

    return $Value.Replace("\", "/")
}

function Clean-NoteName {
    param([AllowNull()][string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return "Unnamed"
    }

    $name = $Value.Trim()
    $name = $name.Replace("[", "(").Replace("]", ")")
    $name = [regex]::Replace($name, '[:#|^]', " ")
    $name = [regex]::Replace($name, '[<>"/\\?*]', "-")
    $name = [regex]::Replace($name, '\s+', " ")
    $name = $name.Trim().Trim(".")

    if ([string]::IsNullOrWhiteSpace($name)) {
        return "Unnamed"
    }

    return $name
}

function To-WikiCsv {
    param([object[]]$Names)

    if (-not $Names -or $Names.Count -eq 0) {
        return ""
    }

    return (($Names | ForEach-Object { "[[$_]]" }) -join ", ")
}

function Escape-YamlValue {
    param([AllowNull()][object]$Value)
    $text = [string]$Value
    return $text.Replace('"', "'")
}

function Get-NodeFolder {
    param(
        [string]$Label,
        [string]$SourceFile
    )

    $pathValue = (Normalize-PathValue $SourceFile).ToLowerInvariant()
    $nameValue = $Label.ToLowerInvariant()

    if ($pathValue.Contains("/controllers/") -or $nameValue.EndsWith("controller") -or $nameValue.Contains("controller.cs")) {
        return "Controllers"
    }

    if ($pathValue.Contains("/services/") -or $nameValue.EndsWith("service") -or $nameValue.StartsWith("iservice") -or $nameValue.Contains("service.cs")) {
        return "Services"
    }

    if ($pathValue.StartsWith("entities/") -or $pathValue.Contains("/entities/")) {
        return "Entities"
    }

    if ($pathValue.Contains("/repositories/") -or $nameValue.EndsWith("repository") -or $nameValue.StartsWith("irepository") -or $nameValue.Contains("repository.cs")) {
        return "Repositories"
    }

    return "Infrastructure"
}

function Add-UniqueName {
    param(
        [AllowNull()][System.Collections.Generic.List[string]]$List,
        [string]$Name
    )

    if ($null -eq $List) {
        return
    }

    if ([string]::IsNullOrWhiteSpace($Name)) {
        return
    }

    if (-not $List.Contains($Name)) {
        $null = $List.Add($Name)
    }
}

function Get-RelationMap {
    return @{}
}

function Add-RelationEntry {
    param(
        [hashtable]$Map,
        [string]$NodeId,
        [string]$Relation,
        [string]$Name
    )

    if ([string]::IsNullOrWhiteSpace($Name)) {
        return
    }

    if (-not $Map.ContainsKey($NodeId)) {
        $Map[$NodeId] = @{}
    }

    if (-not $Map[$NodeId].ContainsKey($Relation)) {
        $Map[$NodeId][$Relation] = New-Object System.Collections.ArrayList
    }

    if (-not $Map[$NodeId][$Relation].Contains($Name)) {
        $null = $Map[$NodeId][$Relation].Add($Name)
    }
}

function Sort-RelationMap {
    param([hashtable]$Map)

    foreach ($nodeKey in @($Map.Keys)) {
        foreach ($relationKey in @($Map[$nodeKey].Keys)) {
            $Map[$nodeKey][$relationKey] = @($Map[$nodeKey][$relationKey] | Sort-Object -Unique)
        }
    }
}

function New-NoteContent {
    param(
        $Node,
        [string]$NoteName,
        [string]$Folder,
        [AllowNull()][hashtable]$OutgoingRelations,
        [AllowNull()][hashtable]$IncomingRelations,
        [string]$GraphCommit
    )

    $label = [string]$Node.label
    $sourceFile = Normalize-PathValue ([string]$Node.source_file)
    $sourceLocation = [string]$Node.source_location
    $nodeId = [string]$Node.id
    $fileType = [string]$Node.file_type
    $community = [string]$Node.community
    $normLabel = [string]$Node.norm_label
    $displaySourceFile = if ([string]::IsNullOrWhiteSpace($sourceFile)) { "n/a" } else { $sourceFile }
    $displaySourceLocation = if ([string]::IsNullOrWhiteSpace($sourceLocation)) { "n/a" } else { $sourceLocation }

    $dependencyNames = [System.Collections.Generic.List[string]]::new()
    foreach ($relation in $dependencyRelations) {
        if ($OutgoingRelations -and $OutgoingRelations.ContainsKey($relation)) {
            foreach ($name in $OutgoingRelations[$relation]) {
                Add-UniqueName -List $dependencyNames -Name $name
            }
        }
    }
    $dependencyNames = @($dependencyNames | Sort-Object)

    $downstreamNames = [System.Collections.Generic.List[string]]::new()
    if ($OutgoingRelations) {
        foreach ($relation in $OutgoingRelations.Keys | Sort-Object) {
            if ($downstreamRelations -contains $relation) {
                foreach ($name in $OutgoingRelations[$relation]) {
                    Add-UniqueName -List $downstreamNames -Name $name
                }
            }
        }
    }
    $downstreamNames = @($downstreamNames | Sort-Object)

    $upstreamNames = [System.Collections.Generic.List[string]]::new()
    if ($IncomingRelations) {
        foreach ($relation in $IncomingRelations.Keys | Sort-Object) {
            if ($upstreamRelations -contains $relation) {
                foreach ($name in $IncomingRelations[$relation]) {
                    Add-UniqueName -List $upstreamNames -Name $name
                }
            }
        }
    }
    $upstreamNames = @($upstreamNames | Sort-Object)

    $lines = [System.Collections.Generic.List[string]]::new()
    $null = $lines.Add("---")
    $null = $lines.Add("title: ""$(Escape-YamlValue $NoteName)""")
    $null = $lines.Add("type: ""$(Escape-YamlValue $folderTypes[$Folder])""")
    $null = $lines.Add("graph_id: ""$(Escape-YamlValue $nodeId)""")
    $null = $lines.Add("label: ""$(Escape-YamlValue $label)""")
    $null = $lines.Add("file_type: ""$(Escape-YamlValue $fileType)""")
    $null = $lines.Add("source_file: ""$(Escape-YamlValue $sourceFile)""")
    $null = $lines.Add("source_location: ""$(Escape-YamlValue $sourceLocation)""")
    $null = $lines.Add("community: ""$(Escape-YamlValue $community)""")
    $null = $lines.Add("norm_label: ""$(Escape-YamlValue $normLabel)""")
    $null = $lines.Add("graph_built_from_commit: ""$(Escape-YamlValue $GraphCommit)""")
    $null = $lines.Add("---")
    $null = $lines.Add("")
    $null = $lines.Add("# $NoteName")
    $null = $lines.Add("")
    $null = $lines.Add('- Category: `' + $Folder + '`')
    $null = $lines.Add('- Label: `' + $label + '`')
    $null = $lines.Add('- Source: `' + $displaySourceFile + '`')
    $null = $lines.Add('- Location: `' + $displaySourceLocation + '`')
    $null = $lines.Add('- Graph Id: `' + $nodeId + '`')
    $null = $lines.Add('- Community: `' + $community + '`')
    $null = $lines.Add("")
    $null = $lines.Add("depends_on:: $(To-WikiCsv $dependencyNames)")
    $null = $lines.Add("upstream:: $(To-WikiCsv $upstreamNames)")
    $null = $lines.Add("downstream:: $(To-WikiCsv $downstreamNames)")
    $null = $lines.Add("")
    $null = $lines.Add("## Dependencies")

    if (@($dependencyNames).Count -gt 0) {
        foreach ($name in $dependencyNames) {
            $null = $lines.Add("- [[$name]]")
        }
    }
    else {
        $null = $lines.Add("- None")
    }

    $null = $lines.Add("")
    $null = $lines.Add("## Downstream Relationships")
    $hasDownstream = $false
    if ($OutgoingRelations) {
        foreach ($relation in $OutgoingRelations.Keys | Sort-Object) {
            if (-not ($downstreamRelations -contains $relation) -or $OutgoingRelations[$relation].Count -eq 0) {
                continue
            }

            $hasDownstream = $true
            $null = $lines.Add("### $($relation.Substring(0,1).ToUpper() + $relation.Substring(1))")
            foreach ($name in $OutgoingRelations[$relation]) {
                $null = $lines.Add('- `' + $relation + '` -> [[' + $name + ']]')
            }
            $null = $lines.Add("")
        }
    }

    if (-not $hasDownstream) {
        $null = $lines.Add("- None")
        $null = $lines.Add("")
    }

    $null = $lines.Add("## Upstream Relationships")
    $hasUpstream = $false
    if ($IncomingRelations) {
        foreach ($relation in $IncomingRelations.Keys | Sort-Object) {
            if (-not ($upstreamRelations -contains $relation) -or $IncomingRelations[$relation].Count -eq 0) {
                continue
            }

            $hasUpstream = $true
            $null = $lines.Add("### $($relation.Substring(0,1).ToUpper() + $relation.Substring(1))")
            foreach ($name in $IncomingRelations[$relation]) {
                $null = $lines.Add('- [[' + $name + ']] -> `' + $relation + '`')
            }
            $null = $lines.Add("")
        }
    }

    if (-not $hasUpstream) {
        $null = $lines.Add("- None")
        $null = $lines.Add("")
    }

    $null = $lines.Add("## Note")
    $null = $lines.Add('Generated from `graphify-out/graph.json` for Obsidian Graph View, Juggl, Excalibrain, and Dataview.')
    $null = $lines.Add("")

    return ($lines -join "`n")
}

$graph = Get-Content $resolvedInput -Raw | ConvertFrom-Json
$nodes = @($graph.nodes)
$links = @($graph.links)
$graphCommit = [string]$graph.built_at_commit
if ([string]::IsNullOrWhiteSpace($graphCommit)) {
    $graphCommit = "unknown"
}

$nodeById = @{}
foreach ($node in $nodes) {
    $nodeById[[string]$node.id] = $node
}

$incomingMethodOwner = @{}
foreach ($link in $links) {
    if ([string]$link.relation -ne "method") {
        continue
    }

    $sourceId = [string]$link.source
    $targetId = [string]$link.target
    if ($nodeById.ContainsKey($sourceId)) {
        $incomingMethodOwner[$targetId] = [string]$nodeById[$sourceId].label
    }
}

$idToName = @{}
$usedNames = @{}
foreach ($node in $nodes) {
    $label = [string]$node.label
    $nodeId = [string]$node.id
    if ($label.StartsWith(".") -and $incomingMethodOwner.ContainsKey($nodeId)) {
        $methodName = $label.TrimStart(".")
        $ownerName = Clean-NoteName $incomingMethodOwner[$nodeId]
        $candidate = Clean-NoteName "$ownerName.$methodName"
    }
    else {
        $candidate = Clean-NoteName $(if ([string]::IsNullOrWhiteSpace($label)) { $nodeId } else { $label })
    }

    if ($usedNames.ContainsKey($candidate)) {
        $usedNames[$candidate] = [int]$usedNames[$candidate] + 1
        $finalName = "$candidate ($($usedNames[$candidate]))"
    }
    else {
        $usedNames[$candidate] = 1
        $finalName = $candidate
    }

    $idToName[$nodeId] = $finalName
}

$outgoingMap = Get-RelationMap
$incomingMap = Get-RelationMap
foreach ($link in $links) {
    $sourceId = [string]$link.source
    $targetId = [string]$link.target
    $relation = [string]$link.relation

    if ([string]::IsNullOrWhiteSpace($sourceId) -or [string]::IsNullOrWhiteSpace($targetId) -or [string]::IsNullOrWhiteSpace($relation)) {
        continue
    }

    if (-not $idToName.ContainsKey($sourceId) -or -not $idToName.ContainsKey($targetId) -or $sourceId -eq $targetId) {
        continue
    }

    Add-RelationEntry -Map $outgoingMap -NodeId $sourceId -Relation $relation -Name $idToName[$targetId]
    Add-RelationEntry -Map $incomingMap -NodeId $targetId -Relation $relation -Name $idToName[$sourceId]
}

Sort-RelationMap -Map $outgoingMap
Sort-RelationMap -Map $incomingMap

if ($Clean -and (Test-Path $resolvedOutput)) {
    Remove-Item -LiteralPath $resolvedOutput -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $resolvedOutput | Out-Null
foreach ($folder in $folders) {
    New-Item -ItemType Directory -Force -Path (Join-Path $resolvedOutput $folder) | Out-Null
}

$counts = @{}
$folderNotes = @{}
foreach ($folder in $folders) {
    $counts[$folder] = 0
    $folderNotes[$folder] = [System.Collections.Generic.List[string]]::new()
}

foreach ($node in $nodes) {
    $nodeId = [string]$node.id
    $noteName = $idToName[$nodeId]
    $folder = Get-NodeFolder -Label ([string]$node.label) -SourceFile ([string]$node.source_file)
    $outgoingRelations = if ($outgoingMap.ContainsKey($nodeId)) { $outgoingMap[$nodeId] } else { $null }
    $incomingRelations = if ($incomingMap.ContainsKey($nodeId)) { $incomingMap[$nodeId] } else { $null }
    $content = New-NoteContent -Node $node -NoteName $noteName -Folder $folder -OutgoingRelations $outgoingRelations -IncomingRelations $incomingRelations -GraphCommit $graphCommit
    $targetFile = Join-Path (Join-Path $resolvedOutput $folder) ($noteName + ".md")
    Set-Content -LiteralPath $targetFile -Value $content -Encoding UTF8
    $counts[$folder] = [int]$counts[$folder] + 1
    $null = $folderNotes[$folder].Add($noteName)
}

$indexLines = [System.Collections.Generic.List[string]]::new()
$null = $indexLines.Add("---")
$null = $indexLines.Add('title: "Graphify Obsidian Index"')
$null = $indexLines.Add('type: "Index"')
$null = $indexLines.Add("graph_built_from_commit: ""$(Escape-YamlValue $graphCommit)""")
$null = $indexLines.Add("---")
$null = $indexLines.Add("")
$null = $indexLines.Add("# Graphify Obsidian Export")
$null = $indexLines.Add("")
$null = $indexLines.Add('- Output folder: `obsidian-graph`')
$null = $indexLines.Add('- Nodes exported: `' + $nodes.Count + '`')
$null = $indexLines.Add('- Relationships exported: `' + $links.Count + '`')
$null = $indexLines.Add('- Graph built from commit: `' + $graphCommit + '`')
$null = $indexLines.Add("")
$null = $indexLines.Add("## Folders")
foreach ($folder in $folders) {
    $null = $indexLines.Add("- [[${folder}/_Index|$folder]] ($($counts[$folder]))")
}
$null = $indexLines.Add("")
$null = $indexLines.Add("## Dataview")
$null = $indexLines.Add('```dataview')
$null = $indexLines.Add("TABLE type, source_file, community")
$null = $indexLines.Add('FROM ""')
$null = $indexLines.Add("WHERE graph_id")
$null = $indexLines.Add("SORT file.name ASC")
$null = $indexLines.Add('```')
$null = $indexLines.Add("")
$rootIndexPath = Join-Path $resolvedOutput "_Index.md"
$rootIndexContent = $indexLines -join "`n"
Set-Content -LiteralPath $rootIndexPath -Value $rootIndexContent -Encoding UTF8

foreach ($folder in $folders) {
    $folderIndexLines = [System.Collections.Generic.List[string]]::new()
    $null = $folderIndexLines.Add("---")
    $null = $folderIndexLines.Add("title: ""$(Escape-YamlValue $folder)""")
    $null = $folderIndexLines.Add('type: "FolderIndex"')
    $null = $folderIndexLines.Add("---")
    $null = $folderIndexLines.Add("")
    $null = $folderIndexLines.Add("# $folder")
    $null = $folderIndexLines.Add("")
    $null = $folderIndexLines.Add("item_count:: $($folderNotes[$folder].Count)")
    $null = $folderIndexLines.Add("")
    $null = $folderIndexLines.Add("## Notes")

    $sortedNotes = $folderNotes[$folder] | Sort-Object
    if (@($sortedNotes).Count -gt 0) {
        foreach ($name in $sortedNotes) {
            $null = $folderIndexLines.Add("- [[$name]]")
        }
    }
    else {
        $null = $folderIndexLines.Add("- None")
    }

    $null = $folderIndexLines.Add("")
    $folderIndexPath = Join-Path (Join-Path $resolvedOutput $folder) "_Index.md"
    $folderIndexContent = $folderIndexLines -join "`n"
    Set-Content -LiteralPath $folderIndexPath -Value $folderIndexContent -Encoding UTF8
}

Write-Host "Exported $($nodes.Count) notes and $($links.Count) relationships to $resolvedOutput"
Write-Host "Graph built from commit: $graphCommit"
foreach ($folder in $folders) {
    Write-Host ("{0}: {1}" -f $folder, $counts[$folder])
}
