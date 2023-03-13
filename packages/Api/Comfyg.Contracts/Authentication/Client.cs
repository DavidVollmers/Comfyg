﻿namespace Comfyg.Contracts.Authentication;

public sealed class Client : IClient
{
    public string ClientId { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;

    public string FriendlyName { get; set; } = null!;
}