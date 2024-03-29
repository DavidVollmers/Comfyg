﻿using System.Security.Cryptography;
using Comfyg.Store.Contracts;

namespace Comfyg.Store.Authentication.Abstractions;

public interface IClientService
{
    Task<IClient?> GetClientAsync(string clientId, CancellationToken cancellationToken = default);

    Task<byte[]> ReceiveClientSecretAsync(IClient client, CancellationToken cancellationToken = default);

    Task<IClient> CreateSymmetricClientAsync(IClient client, CancellationToken cancellationToken = default);

    Task<IClient> CreateAsymmetricClientAsync(IClient client, RSA publicKey, CancellationToken cancellationToken = default);

    Task<Stream?> GetEncryptionKeyAsync(IClient client, CancellationToken cancellationToken = default);

    Task SetEncryptionKeyAsync(IClient client, Stream stream, CancellationToken cancellationToken = default);
}
