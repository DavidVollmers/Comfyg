# Security

The Comfyg store implements multiple layers of security to protect its values.

The general concept of authentication and identification is done by using "Clients".
Each client has its own unique identifier (Client ID) and a secret (Client Secret) used for authentication.

Depending on the use case there are different ways on how to establish this concept.

## System Client

The system client is a special client which has elevated privileges on the Comfyg store. There can only be one system
client for each Comfyg store.

When setting up a new Comfyg store the system client is required to setup new clients and should be removed after
initializing the Comfyg store.

The system client is defined by two environment variables which the Comfyg store will use. If one of the environment
variables is not set correctly the system client cannot be used.

| Environment Variable        | Description                             |
|-----------------------------|-----------------------------------------|
| `COMFYG_SystemClientId`     | The client ID of the system client.     |
| `COMFYG_SystemClientSecret` | The client secret of the system client. |

You can read more about all supported environment variables [here](hosting.md#environment-variables).

> [!IMPORTANT]
> The system client can always access, edit and delete all values of the Comfyg store. It should only be used for client
> setup and in emergency cases.

You can create new clients using the `comfyg setup client` command. You can read more about
it [here](cli/command_setup_client.md).

## Symmetric Clients

Per default when creating a new client on a Comfyg store it will be created as a symmetric client. This means that the
Comfyg store will automatically generate a 64 bytes long secret for the client.

The term "symmetric" is used here because both sides of the connection (the client and the Comfyg store) know this
secret.

> [!NOTE]
> Symmetric clients can only be created by the system client.

## Asymmetric Clients

An option to keep the secret of a client only on the client side is to create it as an asymmetric client. This can be
done by generating a cryptographic key pair on the client side where only the public key will be uploaded to the Comfyg
store for verification purposes.

You can generate this cryptographic key pair using the tool [OpenSSL](https://www.openssl.org):

```shell
openssl genrsa -out keys.pem
```

You can read more about the used command [here](https://www.openssl.org/docs/man1.0.2/man1/genrsa.html).

This will create a RFC 7468 PEM-encoded file which contains both the public and the private key portion of the client
secret.

### End-to-end Encryption

Another upside of using asymmetric clients is the possibility to use end-to-end encryption (E2EE). This means that
values are never stored in clear text on the Comfyg store and cannot be intercepted during the connection.

Every asymmetric client is capable of E2EE out of the box but there are some restrictions to keep in mind.

#### Enabling Encryption

To enable en- and decryption of Comfyg values you must add the `Encryption=true` flag in the connection string of your
client:

```
"Endpoint=...;ClientId=myclient;ClientSecret=path/to/keys.pem;Encryption=true;"
```

#### Sharing encrypted Values

Since each client uses a randomly generated key to en- and decrypt values they cannot be used with other clients per
default.

To be able to share encrypted Values between multiple clients they need to share the same encryption key. This can only
be controlled in the creation process of the client:

- Whenever an asymmetric client is created using the system client a new encryption key will be generated.
- If an asymmetric client is used to create another asymmetric client it will share its encryption key.
- Symmetric clients cannot be used to create an asymmetric client since they have no own encryption key.

## Encryption

The Comfyg store encrypts all secret values (either values added with the `comfyg add secret` command or symmetric
client secrets) before storing them.

This is done using the encryption keys provided by the initial setup of the Comfyg store. You can read more about
this [here](hosting.md#environment-variables).

> [!NOTE]
> This is no end-to-end encryption and is only used as another security layer for sensitive information. End-to-end
> encryption is only supported by asymmetric clients.

## Permissions

Last but not least are permissions which define which client can access, edit and delete which values in the Comfyg
store.

The following permissions can be assigned:

| Permission | Description                                              |
|------------|----------------------------------------------------------|
| `read`     | Allow reading values.                                    |
| `write`    | Allow updating values.                                   |
| `delete`   | Allow deleting values.                                   |
| `permit`   | Allow assigning permissions for values to other clients. |

Permissions are assigned per value to clients which can be done by using the `comfyg set permissions` command. You can
read more about it [here](cli/command_set_permissions.md).
