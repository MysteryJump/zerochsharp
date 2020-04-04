# How to create plugin

## Plugin Language

This application supports only C# Script (.csx) for now, however we will support F# script (.fsx) and Lua Script (.lua) in the future.

## Plugin Defintion

Plugin needs plugin.json definated by plugin.scheme.json and at least one plugin code file subscribed in plugin.json.
Plugin requires plugin.json at the top level of archived zip file.

## Codes

In the plugin code, codes follows the starndard rules of C# Script.
And PluginSetting which define at plugin.json and Board, Thread, Response. (Session info is not available for now)
Plugin can rewrite the data of Thread and Response properties and the data is applied as it is.

## Definition of Board, Thread, Response (from Source code and only summary)

### Board (Read only)

- string BoardKey
- string BoardName
- string BoardDefaultName
- int BoardSambaTime
- string BoardSubTitle
- string BoardDeleteName

### Thread

- string Title
- DateTime Created
- DateTime Modified
- string Author
- int ResponseCount
- bool Archived
- bool Stopped

### Response

- string Author
- DateTime Created
- string Body
- string Mail
- string Name
- string HostAddress
- bool IsAboned

## See also

This example folder contains omikuji plugin example and zip file is deployable.
