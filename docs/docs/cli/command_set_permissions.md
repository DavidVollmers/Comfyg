# comfyg set permissions

[!INCLUDE [Connection Required](connection_required_note.md)]

## Name

`comfyg set permissions` - Sets permissions on permitted key-value pairs for another client.

## Synopsis

```shell
comfyg set permissions [config|secret|setting] <CLIENT_ID> [<KEY>] [-p|--permissions <PERMISSIONS>]

comfyg set permissions [config|secret|setting] -?|-h|--help
```

## Description

The `comfyg set permissions` command sets permissions on permitted key-value pairs of the currently connected client for
another client.

There are three sub commands, one for each supported Comfyg value type. Otherwise it will assign permissions to **all**
permitted key-value pairs.

- `config`

  Sets the permission on a configuration key-value pair.

- `secret`

  Sets the permission on a secret key-value pair.

- `setting`

  Sets the permission on a setting key-value pair.

> [!NOTE]
> You can only set permissions on key-value pairs for which the currently connected client has the `permit` permission.
>
> You can read more about connections & security [here](../TODO.md).

## Arguments

- `CLIENT_ID`

  The ID of the client to set the permissions for.

- `KEY`

  The key of the key-value pair to set the permission for.

## Options

- `-p|--permissions <PERMISSIONS>`

  The kind of permissions which will be set for the client. Defaults to `read`.

  The following permissions can be set:

  - `read`
  
    Permission to read the key-value pair(s).

  - `write`

    Permission to update the key-value pair(s).

  - `delete`

    Permission to delete the key-value pair(s).

  - `permit`

    Permission to set permissions for other clients on the key-value pair(s).

## Examples

- Set `write` and `read` permissions for all permitted key-value pairs for another client with the ID "test":

  ```shell
  comfyg set permissions "test" -p write read
  ```
