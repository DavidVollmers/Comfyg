# comfyg add

[!INCLUDE [Connection Required](connection_required_note.md)]

## Name

`comfyg add` - Adds a key-value pair to the connected Comfyg store.

## Synopsis

```shell
comfyg add config|secret|setting <KEY> <VALUE>

comfyg add [config|secret|setting] -?|-h|--help
```

## Description

The `comfyg add` command adds a key-value pair to the connected Comfyg store.

There are three sub commands, one for each supported Comfyg value type:

- `config` 

  Adds the key-value pair as a configuration value.
 
- `secret`

  Adds the key-value pair as a secret value.

- `setting`

  Adds the key-value pair as a setting value.

## Arguments

- `KEY`

  The key of the key-value pair.

- `VALUE`

  The value of the key-value pair.

## Examples

- Add a configuration value for the lifespan of tokens in your project:

  ```shell 
  comfyg add config "TokenLifespanInHours" "24"
  ```

- Add a secret value for the connection string to your SQL database:

  ```shell
  comfyg add secret "SQLConnectionString" "Server=MySQLServer;Database=MyDatabase;User Id=sa;Password=Password!;"
  ```

- Add a setting value to toggle maintenance mode in your application:

  ```shell
  comfyg add setting "MaintenanceMode" "true"
  ```
