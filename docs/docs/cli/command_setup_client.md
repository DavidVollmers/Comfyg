# comfyg setup client

[!INCLUDE [Connection Required](connection_required_note.md)]

> [!NOTE]
> This command requires a connection with the Comfyg store system client.
> 
> You can read more about the system client [here](../TODO.md)

## Name

`comfyg setup client` - Registers a new client on the connected Comfyg store.

## Synopsis

```shell
comfyg setup client <CLIENT_ID> <FRIENDLY_NAME>

comfyg setup client -?|-h|--help
```

## Description

The `comfyg setup client` command registers a new client on the connected Comfyg store.

If the setup succeeds, the CLI will print out the information of the created client and its client secret.

> [!IMPORTANT]
> Make sure to copy and save the client secret somewhere secure since you cannot access it afterwards.
> 
> If you loose the client secret you can no longer use the created client.

## Arguments

- `CLIENT_ID`

  The ID of the client to create. This must be unique for the connected Comfyg store.

- `FRIENDLY_NAME`

  The user friendly display name which is used for the created client.

## Examples

- Register a new client with the ID "test" and the friendly name "Test Client":

  ```shell
  comfyg setup client "test" "Test Client"
  ```
