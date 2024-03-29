﻿using System.Security.Cryptography;
using Comfyg.Store.Core.Abstractions.Secrets;
using Comfyg.Store.Core.Secrets;

namespace Comfyg.Store.Core.Tests.Secrets;

public class EncryptionBasedSecretServiceTests
{
    [Fact]
    public async Task Test_Roundtrip()
    {
        var encryptionKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var value = Guid.NewGuid().ToString();

        ISecretService secretService = new EncryptionBasedSecretService(encryptionKey);

        var encrypted = await secretService.ProtectSecretValueAsync(value);
        Assert.NotNull(encrypted);
        Assert.NotEqual(value, encrypted);

        var decrypted = await secretService.UnprotectSecretValueAsync(encrypted);
        Assert.NotNull(decrypted);
        Assert.Equal(value, decrypted);
    }
}
