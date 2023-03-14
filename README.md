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

- Add option for wildcard permissions
- Add options to manage configuration, settings and secrets
- Add client side exception handling
- Build Comfyg SDK for IConfiguration usage
- Add `comfyg setup localhost` command
- Add Import/Export capabilities
- Documentation
- Tests
