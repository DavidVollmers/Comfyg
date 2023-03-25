# Comfyg.Api

[![](https://img.shields.io/docker/v/dvol/comfyg/latest?style=flat-square)](https://hub.docker.com/r/dvol/comfyg)

## Local Development

### Azure Storage Emulator

Install and start the [Azurite emulator](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) for
local Azure Storage development.

```shell
npm i -g azurite
azurite --silent --location c:\azurite --debug c:\azurite\debug.log
```

### Set User Secrets

Use the [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/) to configure the `Comfyg.Api` project.

```shell
dotnet user-secrets set AuthenticationAzureTableStorageConnectionString "UseDevelopmentStorage=true"
dotnet user-secrets set AuthenticationEncryptionKey "YOUR_ENCRYPTION_KEY"

dotnet user-secrets set SystemAzureTableStorageConnectionString "UseDevelopmentStorage=true"
dotnet user-secrets set SystemEncryptionKey "YOUR_ENCRYPTION_KEY"
dotnet user-secrets set SystemClientId "system"
dotnet user-secrets set SystemClientSecret "YOUR_SYSTEM_PASSWORD"
```

> `YOUR_ENCRYPTION_KEY` and `YOUR_SYSTEM_PASSWORD` should be replaced with a password (base 64, at least 128 bit) of
> your choice. Keep in mind changing `YOUR_ENCRYPTION_KEY` mid development will break all clients created using the old
> value.
