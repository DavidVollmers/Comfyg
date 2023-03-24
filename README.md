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

For local setup of the Comfyg API you can use the CLI. Just execute the below command and it will guide you through the setup process.

```shell
comfyg setup localhost
```

Once you have setup your local Comfyg API you will receive a connection string from the CLI:

```shell
Successfully started Comfyg API!

You can connect to your local Comfyg API using the following connection string:
Endpoint=http://localhost:32771;ClientId=system;ClientSecret=cbO+N4fgq7mOB813KuOfow0nfKFW+VyS3k4boosUzozn9vmOqvk32QCpxO1eQfxMxKcItHkYX7YUT9uSbP/84g==;
```

The last line contains the connection string to your local Comfyg API using the configured system client. Copy this value so you can use it in the next step. If you loose your connection string you can always just setup a new Comfyg API using the CLI.

You can read more about connections & security [here](docs/Connections.md).

### Create your Comfyg

Now that you have your own Comfyg API running on your local machine you can connect to it using the CLI:

```shell
comfyg connect "YOUR_CONNECTION_STRING"
```

After this you can start adding your Comfyg values using the `comfyg add` command.

#### Configuration Values

Configuration values are the most basic things for your program to work. Things which in most cases only need to be retrieved on startup. For example the lifespan of issued tokens:

```shell
comfyg add config "TokenLifespanInHours" "24"
```

#### Setting Values

Settings are like configuration values but expected to be able to change at runtime. For example a maintenance mode which can be toggled on demand:

```shell
comfyg add setting "MaintenanceMode" "true"
```

#### Secret Values

Last but not least of course the most important part: Secrets. These values as they imply are usually security sensitive values which should be handled carefully. For example the connection string to your SQL database:

```shell
comfyg add secret "SQLConnectionString" "Server=MySQLServer;Database=MyDatabase;User Id=sa;Password=Password!;"
```

Secrets are handled separately because they need to be encrypted. Currently you can either choose to use the Comfyg API internal encryption or Azure Key Vault. For long-term security we recommend the latter one. 

You can read more about this [here](docs/Encryption.md).

### Use your Comfyg

Now that you have setup your Comfyg API and put in some values you will want to use it in your program.

You can do this by adding the [Comfyg package](packages/Comfyg/README.md) to your project:

```shellc
dotnet add package Comfyg
```

To consume your Comfyg API you will have to setup the Comfyg configuration provider.

Here is an example on how to do this in an ASP.NET project:

```csharp
using Comfyg;

// ...

builder.Configuration.AddComfyg(options => { options.Connect("YOUR_CONNECTION_STRING"); });
```

> The connection string should be stored as user secret or environment variable and never be committed.

You can also have a look at our example projects [here](examples/).

#### Access your Comfyg values

To access your Comfyg values you can simply use the `IConfiguration` object in your program like you do with every other configuration provider:

```csharp
var tokenLifespanInHours = configuration.GetValue<int>("TokenLifespanInHours");

var isMaintenanceMode = configuration.GetValue<bool>("MaintenanceMode");

var sqlConnectionString = configuration["SQLConnectionString"];
```

#### Detecting Changes

Per default the Comfyg configuration provider will detect changes only on setting values every 5 minutes.

You can however configure change detection for all three types of comfyg values by configuring the `AddComfyg` method call:

```csharp 
builder.Configuration.AddComfyg(options => { 
    options.Connect("YOUR_CONNECTION_STRING");
    
    // detect changes on secrets every 30 minutes
    options.Secrets.DetectChanges(TimeSpan.FromMinutes(30));
});
```

You can read more about all options [here](docs/TODO.md).

## TODOs

These are things which still need to be done.

- Add option to use Azure KeyVault instead of system encryption
- Make systemId configurable
- Add option for wildcard permissions
- Add client side exception handling
- Add Import/Export capabilities
- Support other authentication options for Azure Table Storage
- Documentation
