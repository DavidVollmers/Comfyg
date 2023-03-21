# Comfyg.Cli

[![](https://img.shields.io/nuget/vpre/Comfyg.Cli?style=flat-square)](https://www.nuget.org/packages/Comfyg.Cli)
[![](https://img.shields.io/github/v/release/DavidVollmers/Comfyg?include_prereleases&style=flat-square)](https://github.com/DavidVollmers/Comfyg/releases)
[![](https://img.shields.io/github/license/DavidVollmers/Comfyg?style=flat-square)](https://github.com/DavidVollmers/Comfyg/blob/main/LICENSE.txt)

> Comfy Command-Line Interface

## Installation

```shell
dotnet add package Comfyg.Cli
```

Visit [nuget.org](https://www.nuget.org/packages/Comfyg.Cli) for more information.


## Local Development

### Build & Test

```shell
dotnet new tool-manifest
```

```shell
dotnet tool uninstall Comfyg.Cli
dotnet pack
dotnet tool install --add-source ./bin/Debug Comfyg.Cli --prerelease
```

> You must commit and push your changes before this will update your local dotnet tool.

You can read more about hot to use local .NET tools [here](https://learn.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use).
