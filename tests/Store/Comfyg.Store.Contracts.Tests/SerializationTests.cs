using System.Text.Json;
using Comfyg.Tests.Common.Contracts;

namespace Comfyg.Store.Contracts.Tests;

public class SerializationTests
{
    [Fact]
    public void Test_IComfygValue_Roundtrip()
    {
        IConfigurationValue configurationValue = new TestConfigurationValue { Key = "key1", Value = "value1" };

        var serializedConfigurationValue = JsonSerializer.Serialize(configurationValue);
        var deserializedConfigurationValue =
            JsonSerializer.Deserialize<IConfigurationValue>(serializedConfigurationValue);
        Assert.NotNull(deserializedConfigurationValue);
        Assert.Equal("key1", deserializedConfigurationValue!.Key);
        Assert.Equal("value1", deserializedConfigurationValue.Value);
    }

    [Fact]
    public void Test_IComfygValue_Enumerable_Roundtrip()
    {
        IEnumerable<IConfigurationValue> configurationValues = new[]
        {
            new TestConfigurationValue { Key = "key1", Value = "value1" }
        };

        var serializedConfigurationValue = JsonSerializer.Serialize(configurationValues);
        var deserializedConfigurationValue =
            JsonSerializer.Deserialize<IEnumerable<IConfigurationValue>>(serializedConfigurationValue);
        Assert.NotNull(deserializedConfigurationValue);
        Assert.Collection(deserializedConfigurationValue,
            result =>
            {
                Assert.NotNull(result);
                Assert.Equal("key1", result.Key);
                Assert.Equal("value1", result.Value);
            });
    }
}
