﻿namespace Comfyg.Authentication.Abstractions;

public interface IClientService
{
    Task<IClient> GetClientAsync(string clientId);

    Task<string> ReceiveClientSecretAsync(IClient client);
}