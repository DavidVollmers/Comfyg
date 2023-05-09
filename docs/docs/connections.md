# Connections

To connect your service or application to a Comfyg store you want to establish a connection first. This is done using a
connection string containing the information on how to establish the connection.

Every connection string is a semicolon separated list of the following information:

| Information    | Description                                                                                                                                                                                                       | Required | Example                        |
|----------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------|--------------------------------|
| `Endpoint`     | The endpoint URI of the Comfyg store to connect to.                                                                                                                                                               | Yes      | `https://example.comfyg.store` |
| `ClientId`     | The identifier of the client to establish the connection with.                                                                                                                                                    | Yes      | `exampleClientId`              |
| `ClientSecret` | The client secret used for authentication. This can either be a symmetric client secret or a path to a RFC 7468 PEM-encoded file which contains both the public and the private key portion of the client secret. | Yes      | `path/to/keys.pem`             |                              
| `Encryption`   | Boolean flag which defines if values should be de- and encrypted. Only is supported using asymmetric clients. You can read more about this [here](security.md#end-to-end-encryption).                             | No       | `true`                         |

You can read more about clients [here](security.md).

> [!NOTE]
> You can substitute all connection string information using environment variables. You can do this by prefixing the name of the environment variable with a `$`. 
> 
> An example on how to do this is shown [below](#examples).

Connection strings are required whenever you want to connect to a Comfyg store:

- Using the [Comfyg](https://www.nuget.org/packages/Comfyg) NuGet package
- Using the [Comfyg.Client](https://www.nuget.org/packages/Comfyg.Client) NuGet package
- Using the `comfyg connect` command (Read more about it [here](cli/command_connect.md))
- Using the [@comfyg/client](https://www.npmjs.com/package/@comfyg/client) NPM package

## Examples

- A connection string for a symmetric client:

  ```
  Endpoint=https://example.comfyg.store;ClientId=exampleClientId;ClientSecret=buNUEUhJAyYoM8cjNoXQZslQIoMBoHiuA9IEcJYa2lU9jJG1ZCH+/c7PW20x7b0NvwnxEucWFCwrpw7TFoMj6Q==;
  ```

- A connection string for an asymmetric client using end-to-end encryption:

  ```
  Endpoint=https://example.comfyg.store;ClientId=exampleClientId;ClientSecret=path/to/keys.pem;Encryption=true;
  ```
  
- A connection string for an asymmetric client using an environment variable which points to the client secret:

  ```
  Endpoint=https://example.comfyg.store;ClientId=exampleClientId;ClientSecret=$EXAMPLE_CLIENT_SECRET;
  ```
