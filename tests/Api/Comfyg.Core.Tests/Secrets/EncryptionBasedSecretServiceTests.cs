using Comfyg.Core.Abstractions.Secrets;
using Comfyg.Core.Secrets;

namespace Comfyg.Core.Tests.Secrets;

public class EncryptionBasedSecretServiceTests
{
    [Fact]
    public async Task Test_Roundtrip()
    {
        var encryptionKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
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