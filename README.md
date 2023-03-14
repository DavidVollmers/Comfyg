# Comfyg

## Getting Started

### Installation

TODO

### Setup & Configure your Comfyg API

TODO

### Create your first client

```shell
comfyg connect "YOUR_SYSTEM_CLIENT" "YOUR_SYSTEM_CLIENT_SECRET"
comfyg setup client "CLIENT_ID" "CLIENT_SECRET"
```

```shell
comfyg connect "YOUR_CLIENT_ID" "YOUR_CLIENT_SECRET"
comfyg add config "ConfigKey" "ConfigValue"
comfyg add setting "SettingKey" "SettingValue"
comfyg add secret "SecretKey" "SecretValue"
```

```shell
comfyg import config appsettings.json
comfyg export config export.json
```

## TODO

- Add options to manage configuration, settings and secrets
- Add option to use KeyVault instead of system encryption
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
