using System.Text.Json.Serialization;
using Comfyg.Contracts.Configuration;

namespace Comfyg.Contracts.Responses;

public sealed class GetConfigurationValuesResponse
{
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<IConfigurationValue>, IEnumerable<ConfigurationValue>,
            IEnumerable<IComfygValue>>))]
    public IEnumerable<IConfigurationValue> ConfigurationValues { get; }

    public GetConfigurationValuesResponse(IEnumerable<IConfigurationValue> configurationValues)
    {
        ConfigurationValues = configurationValues ?? throw new ArgumentNullException(nameof(configurationValues));
    }
}