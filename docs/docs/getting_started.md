# Getting Started

To try out Comfyg as fast and quick as possible you can follow this guide to set everything up on your local machine.

## Requirements

- [Docker](https://docs.docker.com/get-docker)
- [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet) version 6.0 or higher

You will also need to setup at least
one [Azure Table Storage account](https://learn.microsoft.com/en-us/azure/storage/common/storage-account-create?tabs=azure-portal)
which will be used to store all Comfyg values.

## Installation

First we want to install the Comfyg Command-Line Interface (CLI). We can do this using the [.NET CLI](https://learn.microsoft.com/de-de/dotnet/core/tools/):

```shell
dotnet tool install --global Comfyg.Cli
```

> [!TIP]
> After installation you can invoke the Comfyg CLI using the `comfyg` command in your terminal.

You can read more about the Comfyg CLI [here](cli/index.md).

## Setup & Configure your Comfyg Store

For local setup of the Comfyg store you can use the CLI. Just execute the below command and it will guide you through the setup process.

```shell
comfyg setup localhost
```

Once you have setup your local Comfyg store you will receive a connection string from the CLI:

```shell
Successfully started local Comfyg store
You can connect to your local Comfyg store using the following connection string:
Endpoint=http://localhost:32771;ClientId=system;ClientSecret=cbO+N4fgq7mOB813KuOfow0nfKFW+VyS3k4boosUzozn9vmOqvk32QCpxO1eQfxMxKcItHkYX7YUT9uSbP/84g==;
```

The last line contains the connection string to your local Comfyg store using the configured system client. Copy this value so you can use it in the next step. If you loose your connection string you can always just setup a new Comfyg store using the CLI.

You can read more about security [here](security.md).

## Create your Comfyg

Now that you have your own Comfyg store running on your local machine you can connect to it using the CLI:

```shell
comfyg connect "YOUR_CONNECTION_STRING"
```

After this you can start adding your Comfyg values using the `comfyg add` command.

### Configuration Values

Configuration values are the most basic things for your program to work. Values which in most cases only need to be retrieved on startup. For example the lifespan of issued tokens:

```shell
comfyg add config "TokenLifespanInHours" "24"
```

### Setting Values

Settings are like configuration values but expected to be able to change at runtime. For example a maintenance mode which can be toggled on demand:

```shell
comfyg add setting "MaintenanceMode" "true"
```

### Secret Values

Last but not least of course the most important part: Secrets. These values, as they imply, are usually security sensitive values which should be handled carefully. For example the connection string to your SQL database:

```shell
comfyg add secret "SQLConnectionString" "Server=MySQLServer;Database=MyDatabase;User Id=sa;Password=Password!;"
```

You can read more about how secrets are secured in [here](security.md#encryption).

## Use your Comfyg

Now that you have setup your Comfyg store and put in some values you will want to use it in your program.

You can do this by adding the [Comfyg package](https://nuget.org/packages/Comfyg) to your project:

```shellc
dotnet add package Comfyg
```

To consume your Comfyg store you will have to setup the Comfyg configuration provider.

Here is an example on how to do this in an ASP.NET project:

```csharp
using Comfyg;

// ...

builder.Configuration.AddComfyg(options => { options.Connect("YOUR_CONNECTION_STRING"); });
```

> [!IMPORTANT]
> The connection string should be stored as a [user secret](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variable and never be committed to your repository.

You can also have a look at our example projects [here](https://github.com/DavidVollmers/Comfyg/tree/main/examples).

### Access your Comfyg values

To access your Comfyg values you can simply use the `IConfiguration` object in your program like you do with every other configuration provider:

```csharp
var tokenLifespanInHours = configuration.GetValue<int>("TokenLifespanInHours");

var isMaintenanceMode = configuration.GetValue<bool>("MaintenanceMode");

var sqlConnectionString = configuration["SQLConnectionString"];
```

### Detecting Changes

Per default the Comfyg configuration provider will detect changes only on setting values every 5 minutes.

You can however configure change detection for all three types of comfyg values by configuring the `AddComfyg` method call:

```csharp 
builder.Configuration.AddComfyg(options => { 
    options.Connect("YOUR_CONNECTION_STRING");
    
    // detect changes on secrets every 30 minutes
    options.Secrets.DetectChanges(TimeSpan.FromMinutes(30));
});
```

You can read more about all options [here](xref:Comfyg.ComfygOptions).

## What's next?

After you played around with your local Comfyg store it will be time to move to production, won't it?

First thing you want to do is to setup your Comfyg store in your own hosting environment. You can read more about this [here](hosting.md).

You also will probably want to import existing configuration, settings and secrets you have into your API. You can do this using the Comfyg CLI:

```shell
comfyg import config "path/to/appsettings.json"
```

You can read more about all Comfyg CLI capabilities [here](cli/index.md#usage).

Last but not least you should also think about security and data separation concerns. You can read more about security [here](security.md).
