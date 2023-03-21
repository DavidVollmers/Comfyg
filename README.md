# Comfyg

## Getting Started

### Installation

```shell
dotnet tool install --global Comfyg.Cli
```

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

```shell
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
- Documentation
- Tests

## TODO CoreHelpers

- Remove Newtonsoft.Json dependency
- Move TechnicalIdentifierAttribute to CoreHelpers
- Support other authentication methods (Azure.Identity)
- StoreAsJsonObject for PK or RK (virtual)
- Consistent type constraints
- Cancellation Token support
- Get-only properties
- Redundant storage of non-virtual PK/RK properties
- throw Exception on null as PK/RK when querying
