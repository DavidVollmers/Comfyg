using System.Text.Json.Serialization;
using Comfyg.Contracts.Configuration;

namespace Comfyg.Contracts.Responses;

public sealed class GetConfigurationResponse
{
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<IConfigurationValue>, IEnumerable<ConfigurationValue>,
            IEnumerable<IComfygValue>>))]
    public IEnumerable<IConfigurationValue> ConfigurationValues { get; }

    public GetConfigurationResponse(IEnumerable<IConfigurationValue> configurationValues)
    {
        ConfigurationValues = configurationValues;
    }
}