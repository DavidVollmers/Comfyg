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
dotnet user-secrets set AuthenticationAzureTableStorageConnectionString "UseDevelopmentStorage=true"
dotnet user-secrets set AuthenticationEncryptionKey "YOUR_ENCRYPTION_KEY"
```

> `YOUR_ENCRYPTION_KEY` should be replaced with a password of your choice. Keep in mind changing this mid development
> will break existing clients.
