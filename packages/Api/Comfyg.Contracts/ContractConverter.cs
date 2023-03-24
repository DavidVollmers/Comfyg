using System.Text.Json;
using System.Text.Json.Serialization;

namespace Comfyg.Contracts;

internal class ContractConverter<TContract, TImplementation> : ContractConverter<TContract, TImplementation, TContract>
    where TImplementation : class, TContract
{
}

internal class ContractConverter<TContract, TImplementation, TSerialization> : JsonConverter<TContract>
    where TImplementation : class, TContract
    where TContract : TSerialization
{
    public override TContract? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<TImplementation>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, TContract value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize<TSerialization>(writer, value, options);
    }
}
