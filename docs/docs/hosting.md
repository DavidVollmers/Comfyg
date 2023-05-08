# Hosting

After trying out the Comfyg store on your local machine using the `comfyg setup localhost` (Read more about
it [here](cli/command_setup_localhost.md)) command you will eventually come to the point where you want to host an
instance of the Comfyg store for your staging/production environment.

In the following you will find a step by step guide on how to host your own Comfyg store.

## Docker

Although you can theoretically host the Comfyg store without Docker we strongly recommend you to use
the [official docker image](https://hub.docker.com/r/dvol/comfyg).

You can easily pull the latest image using the [Docker CLI](https://docs.docker.com/engine/reference/commandline/cli/):

```shell
docker pull dvol/comfyg
```

You can read more about the `docker pull` command [here](https://docs.docker.com/engine/reference/commandline/pull/).

## Running the Comfyg store

When you have the Docker image pulled to the hosting machine you can run the Comfyg store by using the Docker CLI again:

```shell
docker run dvol/comfyg
```

This will start the Comfyg store in a new Docker container. The Comfyg store will perform health checks on startup which
generate errors if something is not properly setup.

When you just run the Docker image as described above you will get the following output:

```shell
warn: Comfyg.Store.Api[0]
      No system client configured.
fail: Comfyg.Store.Api[0]
      The Comfyg store system is not healthy.
      System.InvalidOperationException: Neither encryption nor Azure Key Vault is configured. Use either ComfygOptions.UseEncryption or ComfygOptions.UseKeyVault to configure secret handling.
```

These errors and warnings are output because the system is not configured correctly. To configure the system you can use
environment variables.

> [!NOTE]
> You can also disable all health checks on startup by setting the environment variable `COMFYG_DoHealthCheckOnStartup`
> to `false`.

You can read more about the `docker run` command and how to supply environment variables [here](https://docs.docker.com/engine/reference/commandline/run/).

## Environment Variables

To properly configure your Comfyg store system you can use the following environment variables:

| Environment Variable                                     | Description                                                                                                            | Example Value                                  |
|----------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------|------------------------------------------------|
| `COMFYG_SystemClientId`                                  | The client ID of the [system client](security.md#system-client).                                                       | `system`                                       |
| `COMFYG_SystemClientSecret`                              | The client secret of the system client.                                                                                | A 64 byte long base64 encoded string.          |
| `COMFYG_SystemAzureTableStorageConnectionString`         | The connection string for the Azure Table Storage used to store all Comfyg values and their metadata.                  | A valid Azure Table Storage connection string. |
| `COMFYG_SystemEncryptionKey`                             | The base64 encoded encryption key used to encrypt and decrypt all secret values.                                       | A 32 byte long base64 encoded string.          |
| `COMFYG_SystemKeyVaultUri`                               | The URI (URL) of the Azure Key Vault used to store all secret values.                                                  | A valid Azure Key Vault URL.                   |
| `COMFYG_AuthenticationAzureTableStorageConnectionString` | The connection string for the Azure Table Storage used to store all authentication related values. (e.g. Clients)      | A valid Azure Table Storage connection string. |
| `COMFYG_AuthenticationAzureBlobStorageConnectionString`  | The connection string for the Azure Blob Storage used to store all authentication related files (e.g. Client Secrets). | A valid Azure Blob Storage connection string.  |
| `COMFYG_AuthenticationEncryptionKey`                     | The base64 encoded encryption key used to encrypt and decrypt all authentication related secrets.                      | A 32 byte long base64 encoded string.          |
| `COMFYG_AuthenticationKeyVaultUri`                       | The URI (URL) of the Azure Key Vault used to store all authentication secrets.                                         | A valid Azure Key Vault URL.                   |
| `COMFYG_DoHealthCheckOnStartup`                          | Optional flag. If set to `false` no health checks will be performed on startup.                                        | `false`                                        |

> [!WARNING]
> If you want to use Azure Key Vault for the system or authentication secrets you cannot use encryption. Configuring
> both will produce errors.

## Using the Comfyg store

After successful startup of the Comfyg store you connect to it with a client using the `comfyg connect` command.

You can read more about clients [here](security.md).

You can read more about the `comfyg connect` command [here](cli/command_connect.md).
