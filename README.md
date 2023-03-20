# Comfyg

## Getting Started

### Installation

TODO

### Setup & Configure your Comfyg API

#### Local setup

```shell
dotnet comfyg setup localhost
```

#### Docker Image

TODO

### Create your first client

```shell
dotnet comfyg connect "CONNECTION_STRING"
dotnet comfyg setup client "CLIENT_ID" "CLIENT_SECRET"
```

### Create your Comfyg

```shell
dotnet comfyg connect "CONNECTION_STRING"
dotnet comfyg add config "ConfigKey" "ConfigValue"
dotnet comfyg add setting "SettingKey" "SettingValue"
dotnet comfyg add secret "SecretKey" "SecretValue"
```

```shell
dotnet comfyg import config appsettings.json
dotnet comfyg export config export.json
```

### Use your Comfyg

```shell
dotnet add package Comfyg
```

```csharp
using Comfyg;

// ...

builder.Configuration.AddComfyg(options => { options.Connect("CONNECTION_STRING"); });
```

> The connection string should be stored as user secret or environment variable and never be committed.

## TODO

- Add option to use Azure KeyVault instead of system encryption
- Add option for wildcard permissions
- Add client side exception handling
- Add `comfyg setup localhost` command
- Add Import/Export capabilities
- Documentation
- Tests

## TODO CoreHelpers

- Remove Newtonsoft.Json dependency
- Move TechnicalIdentifierAttribute to CoreHelpers
- Support other authentication methods (Azure.Identity)
- StoreAsJsonObject for PK or RK (virtual)
- Consistent type constraints
- Cancellation Token support
