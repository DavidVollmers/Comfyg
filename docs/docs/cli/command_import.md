# comfyg import

[!INCLUDE [Connection Required](connection_required_note.md)]

## Name

`comfyg import` - Imports key-value pairs into the connected Comfyg store.

## Synopsis

```shell
comfyg import config|secrets|settings <INPUT_FILE>

comfyg import [config|secrets|settings] -?|-h|--help
```

## Description

The `comfyg import` command imports key-value pairs into the connected Comfyg store from the provided input file.

The input file must be in JSON format. The import of key-value pairs follows the same behaviour as the adding of key-value pairs using the [comfyg add](command_add.md) command.

## Arguments

- `INPUT_FILE`

  The input file which will be used to import all key-value pairs from.

## Examples

- Import all key-value pairs from the provided input file as configuration values into the connected Comfyg store:

  ```shell
  comfyg import config "appsettings.json"
  ```
