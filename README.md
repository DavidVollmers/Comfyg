# Comfyg

[![](https://img.shields.io/docker/v/dvol/comfyg/latest?style=flat-square)](https://hub.docker.com/r/dvol/comfyg)
[![](https://img.shields.io/nuget/vpre/Comfyg?style=flat-square)](https://www.nuget.org/packages/Comfyg)
[![](https://img.shields.io/github/v/release/DavidVollmers/Comfyg?include_prereleases&style=flat-square)](https://github.com/DavidVollmers/Comfyg/releases)
[![](https://img.shields.io/github/license/DavidVollmers/Comfyg?style=flat-square)](https://github.com/DavidVollmers/Comfyg/blob/main/LICENSE.txt)

> Comfy configuration, settings and secrets management/distribution platform

Comfyg is the answer to the question "Where do I store my configuration? Where my secrets? And what about settings I
want to change at runtime?".

You can compare it
to [Azure App Configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview) but also
supporting secrets and change detection.

## Requirements

- Host the Comfyg API as a Docker Container
- At least one Azure Storage Account

To run Comfyg you need to host the API somewhere. For this you can simply use
the [latest docker image](https://hub.docker.com/r/dvol/comfyg/tags).

You also will need to setup at least
one [Azure Table Storage account](https://learn.microsoft.com/en-us/azure/storage/common/storage-account-create?tabs=azure-portal) which will be used to store all Comfyg values.

There are also options to distribute the storage on multiple accounts or connecting to [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault) for secret management.

## Getting Started

To try out Comfyg as fast and quick as possible follow this guide to set everything up on your local machine.

### Installation

First we want to install the Comfyg Command-Line Inteface (CLI). We can do this using the .NET CLI:

```shell
dotnet tool install --global Comfyg.Cli
```

> After installation you can access the Comfyg CLI using the `comfyg` driver in any terminal.

### Setup & Configure your Comfyg API

```shell
comfyg setup localhost
```

### Create your first client

```shell
comfyg connect "CONNECTION_STRING"
comfyg setup client "CLIENT_ID" "CLIENT_SECRET"
```

### Create your Comfyg

```shell
comfyg connect "CONNECTION_STRING"
comfyg add config "ConfigKey" "ConfigValue"
comfyg add setting "SettingKey" "SettingValue"
comfyg add secret "SecretKey" "SecretValue"
```

### Use your Comfyg

```shellc
dotnet add package Comfyg
```

```csharp
using Comfyg;

// ...

builder.Configuration.AddComfyg(options => { options.Connect("CONNECTION_STRING"); });
```

> The connection string should be stored as user secret or environment variable and never be committed.

#### Access Comfyg values

```csharp
var configValue = configuration["ConfigKey"];
var settingValue = configuration["SettingKey"];
var secretValue = configuration["SecretKey"];
```

## TODO

- Add option to use Azure KeyVault instead of system encryption
- Make systemId configurable
- Add option for wildcard permissions
- Add client side exception handling
- Add Import/Export capabilities
- Support other authentication options for Azure Table Storage
- Documentation
- Tests
