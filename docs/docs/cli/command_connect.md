# comfyg connect

## Name

`comfyg connect` - Establishes a connection to a Comfyg store.

## Synopsis

```shell
comfyg connect <CONNECTION_STRING>

comfyg connect -?|-h|--help
```

## Description

The `comfyg connect` command establishes a connection to a Comfyg store via the provided connection string.

A valid connection string looks like this:

```
Endpoint=http://localhost:32771;ClientId=system;ClientSecret=cbO+N4fgq7mOB813KuOfow0nfKFW+VyS3k4boosUzozn9vmOqvk32QCpxO1eQfxMxKcItHkYX7YUT9uSbP/84g==;
```

It contains the `Endpoint` where the Comfyg store is located and also the `ClientId` and `ClientSecret` of the client to use for authentication.
Depending on the client you choose for connection, you will have different privileges and permissions on the values stored in the Comfyg store.

You can read more about this [here](../TODO.md).

If the connection succeeds it will print out the information of the client used for the connection.
Also the CLI will store the connection string in your user profile so that the connection can be used for other CLI commands.

## Arguments

- `CONNECTION_STRING`

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;The connection string used to connect to the Comfyg store.

## Examples

- Connect to the local Comfyg store from the above example connection string:

```shell
comfyg connect "Endpoint=http://localhost:32771;ClientId=system;ClientSecret=cbO+N4fgq7mOB813KuOfow0nfKFW+VyS3k4boosUzozn9vmOqvk32QCpxO1eQfxMxKcItHkYX7YUT9uSbP/84g==;"
```
