using System.Text.Json.Serialization;
using Comfyg.Contracts.Configuration;

namespace Comfyg.Contracts.Responses;

public sealed class GetConfigurationResponse
{
    [JsonConverter(typeof(ContractConverter<IConfigurationValue[], ConfigurationValue[]>))]
    public IConfigurationValue[] ConfigurationValues { get; }

    public GetConfigurationResponse(IEnumerable<IConfigurationValue> configurationValues)
    {
        ConfigurationValues = configurationValues.ToArray();
    }
}