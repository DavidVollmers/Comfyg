# comfyg setup localhost

## Name

`comfyg setup localhost` - Sets up a new Comfyg store on your local machine.

## Synopsis

```shell
comfyg setup localhost [--authentication-azure-table-storage-connection-string] 
    [--authentication-encryption-key] [-ds|--docker-socket] [-df|--dockerfile]
    [--system-azure-table-storage-connection-string] [--system-client-id]
    [--system-client-secret] [--system-encryption-key] [-v|--version]

comfyg setup localhost -?|-h|--help
```

## Description

The `comfyg setup localhost` sets up a new Comfyg store on your local machine. During the setup the CLI will prompt for all required parameters, if they are not specified as options.

If the setup succeeds, the CLI will output the connection string of the locally setup Comfyg store. You can use this connection string with the [comfyg connect](command_connect.md).

> [!WARNING]
> If you loose the connection string you cannot connect to your local Comfyg store anymore.
> 
> In the worse case you can always setup a new Comfyg store using this command.

The setup, if not specified otherwise, will use the latest version of the [Comfyg store API Docker Image](https://hub.docker.com/r/dvol/comfyg) and cleans up all running containers before starting a new one.

## Options

- `--authentication-azure-table-storage-connection-string`

  A connection string to connect to the Azure Storage Account used to store all authentication related values.

- `--authentication-encryption-key`

  The base64 encoded key used to encrypt all authentication related secrets.

- `-ds|--docker-socket`

  The docker socket to use for creating and running the docker container.

- `-df|--dockerfile`

  The Dockerfile used to build the container from. This can be used for local development of the Comfyg store API or when testing your own Comfyg store API implementation.

- `--system-azure-table-storage-connection-string`

  A connection string to connect to the Azure Storage Account used to store all Comfyg values.

- `--system-client-id`

  The client ID of the Comfyg system client.

- `--system-client-secret`

  The client secret of the Comfyg system client.

- `--system-encryption-key`

  The base64 encoded key used to encrypt all Comfyg secret values.

- `-v|--version`

  The version of the Docker Image to used. Defaults to `azurite`.

## Examples

- Start the setup which will guide through the setup process:

  ```shell
  comfyg setup localhost
  ```
