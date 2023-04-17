# comfyg export

[!INCLUDE [Connection Required](connection_required_note.md)]

## Name

`comfyg export` - Exports key-value pairs from the connected Comfyg store.

## Synopsis

```shell
comfyg export config|secrets|settings <OUTPUT_FILE> [-t|--tags <TAGS>]

comfyg export [config|secrets|settings] -?|-h|--help
```

## Description

The `comfyg export` command exports key-value pairs from the connected Comfyg store into the provided output file.

The output is created in JSON format. If the provided output file already exists the CLI will ask if it should be overwritten.

## Arguments

- `OUTPUT_FILE`

  The output file which will be used to export key-value pairs into.

## Options

- `-t|--tags <TAGS>`

  Export key-value pairs tagged with the provided tags.

## Examples

- Export all configuration values from the connected Comfyg store info the provided JSON file:

  ```shell
  comfyg export config "appsettings.json"
  ```

- Export setting values which are tagged with the "dev" tag:

  ```shell
  comfyg export settings "appsettings.Development.json" -t dev
  ```
