﻿# comfyg add

> [!NOTE]
> This command requires a connection to a Comfyg store. You can establish a connection using the [comfyg connect](command_connect.md) command.

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

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Adds the key-value pair as a configuration value.
 
- `secret`

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Adds the key-value pair as a secret value.

- `setting`

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Adds the key-value pair as a setting value.

## Arguments

- `KEY`

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;The key of the key-value pair.

- `VALUE`

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;The value of the key-value pair.

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