using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts;

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
        return JsonSerializer.Deserialize<TImplementation>(ref reader,
            new JsonSerializerOptions(options) { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    public override void Write(Utf8JsonWriter writer, TContract value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var propertyInfo in typeof(TSerialization).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var ignoreAttribute = propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>(true) != null;
            if (ignoreAttribute) continue;

            writer.WritePropertyName(JsonNamingPolicy.CamelCase.ConvertName(propertyInfo.Name));
            writer.WriteRawValue(JsonSerializer.Serialize(propertyInfo.GetValue(value)));
        }

        writer.WriteEndObject();
    }
}
