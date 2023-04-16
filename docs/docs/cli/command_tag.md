# comfyg tag

[!INCLUDE [Connection Required](connection_required_note.md)]

## Name

`comfyg tag` - Tags a key-valur pair version.

## Synopsis

```shell
comfyg tag config|secret|setting <KEY> <TAG> [-v|--version <VERSION>]

comfyg tag [config|secret|setting] -?|-h|--help
```

## Description

The `comfyg tag` command tags a key-value pair and creates a new tagged version of it.

Tags can be used to categorize or filter key-value pairs. This is powerful when you have multiple applications consuming
the same Comfyg store but requiring different values to be configured.

A real world example are development, testing or staging environments: You can add a tag for each environment which
allows you to configure different values with the same key.

You can read more about how tags work [here](../TODO.md).

There are three sub commands, one for each supported Comfyg value type:

- `config`

  Tags a configuration key-value pair.

- `secret`

  Tags a secret key-value pair.

- `setting`

  Tags a setting key-value pair.

## Arguments

- `KEY`

  The key of the key-value pair.

- `TAG`

  The identifier of the tag.

## Options

- `-v|--version <VERSION>`

  The version of the key-value pair. Defaults to `latest`. You can read more about how versioning works [here](../TODO.md).

## Examples

-  Tag a configuration value for the lifespan of tokens for your testing/development environment:

  ```shell
  comfyg tag config "TokenLifespanInHours" "dev"
  ```
