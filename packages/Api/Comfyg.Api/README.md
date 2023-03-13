# Comfyg.Api

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
dotnet user-secrets set ComfygAuthenticationAzureTableStorageConnectionString "UseDevelopmentStorage=true"
dotnet user-secrets set ComfygAuthenticationEncryptionKey "YOUR_ENCRYPTION_KEY"

dotnet user-secrets set ComfygSystemAzureTableStorageConnectionString "UseDevelopmentStorage=true"
dotnet user-secrets set ComfygSystemClient "system"
dotnet user-secrets set ComfygSystemClientSecret "YOUR_SYSTEM_PASSWORD"
```

> `YOUR_ENCRYPTION_KEY` and `YOUR_SYSTEM_PASSWORD` should be replaced with a password of your choice. Keep in mind
> changing `YOUR_ENCRYPTION_KEY` mid development will all clients created under the old value.
