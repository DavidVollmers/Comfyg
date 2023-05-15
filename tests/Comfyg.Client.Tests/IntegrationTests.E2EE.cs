using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Tests.Common.Contracts;
using Moq;

namespace Comfyg.Client.Tests;

public partial class IntegrationTests
{
    [Fact]
    public async Task Test_EndToEndEncryption()
    {
        var clientId = Guid.NewGuid().ToString();
        var assemblyPath = new FileInfo(Assembly.GetAssembly(typeof(IntegrationTests))!.Location).Directory!.FullName;
        var keysPath = Path.Join(assemblyPath, "test.pem");
        const string friendlyName = "Test Client";
        var client = new TestClient {ClientId = clientId, FriendlyName = friendlyName, IsAsymmetric = true};
        const string decryptedValue1 = "value1";
        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor();
        var encryptedValue1 =
            await EncryptValueAsync(encryptor, decryptedValue1) + "." + Convert.ToBase64String(aes.IV);
        var configurationValues = new[]
        {
            new TestConfigurationValue {Key = "key1", Value = encryptedValue1, IsEncrypted = true},
            new TestConfigurationValue {Key = "key2", Value = "value2"}
        };
        
        using var rsa = RSA.Create();
        rsa.ImportFromPem(await File.ReadAllTextAsync(keysPath));
        var publicKey = rsa.ExportRSAPublicKey();
        var privateKey = rsa.ExportRSAPrivateKey();

        using var privateRsaOnly = RSA.Create();
        privateRsaOnly.ImportRSAPrivateKey(privateKey, out _);
        var encryptedKey = privateRsaOnly.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
        var encryptedKeyStream = new MemoryStream(encryptedKey);

        using var httpClient = _factory.CreateClient();

        var connectionString =
            $"Endpoint={httpClient.BaseAddress};ClientId={clientId};ClientSecret={keysPath};Encryption={true}";
        using var comfygClient = new ComfygClient(connectionString, httpClient);

        _factory.Mock<IClientService>(mock =>
        {
            mock.Setup(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(client);
            mock.Setup(cs =>
                    cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(publicKey);
            mock.Setup(cs => cs.GetEncryptionKeyAsync(It.IsAny<IClient>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(encryptedKeyStream);
        });

        _factory.Mock<IValueService<IConfigurationValue>>(mock =>
        {
            mock.Setup(vs => vs.GetLatestValuesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(configurationValues.ToAsyncEnumerable);
        });

        var results = await comfygClient.GetValuesAsync<IConfigurationValue>().ToListAsync();
        Assert.NotNull(results);
        Assert.Collection(results,
            result =>
            {
                Assert.NotNull(result);
                Assert.Equal("key1", result.Key);
                Assert.Equal(decryptedValue1, result.Value);
                Assert.False(result.IsEncrypted);
            },
            result =>
            {
                Assert.NotNull(result);
                Assert.Equal("key2", result.Key);
                Assert.Equal("value2", result.Value);
                Assert.False(result.IsEncrypted);
            });

        _factory.Mock<IClientService>(mock =>
        {
            mock.Verify(cs => cs.GetClientAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            mock.Verify(
                cs => cs.ReceiveClientSecretAsync(It.Is<IClient>(c => c.ClientId == clientId),
                    It.IsAny<CancellationToken>()), Times.Exactly(2));
            mock.Verify(cs =>
                    cs.GetEncryptionKeyAsync(It.Is<IClient>(c => c.ClientId == clientId),
                        It.IsAny<CancellationToken>()),
                Times.Once);
        });

        _factory.Mock<IValueService<IConfigurationValue>>(mock =>
        {
            mock.Verify(vs =>
                vs.GetLatestValuesAsync(It.Is<string>(s => s == clientId), It.IsAny<CancellationToken>()), Times.Once);
        });

        await encryptedKeyStream.DisposeAsync();
    }

    private static async Task<string> EncryptValueAsync(ICryptoTransform encryptor, string rawValue)
    {
        using var stream = new MemoryStream();
        var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
        var writer = new StreamWriter(crypto);
        await writer.WriteAsync(rawValue).ConfigureAwait(false);
        await writer.DisposeAsync().ConfigureAwait(false);
        await crypto.DisposeAsync().ConfigureAwait(false);

        return Convert.ToBase64String(stream.ToArray());
    }
}
