# Comfyg.Cli

## Local Development

### Build & Test

```shell
dotnet tool uninstall Comfyg.Cli
dotnet pack
dotnet tool install --add-source ./bin/Debug Comfyg.Cli --prerelease
```

You can read more about hot to use local .NET tools [here](https://learn.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use).
