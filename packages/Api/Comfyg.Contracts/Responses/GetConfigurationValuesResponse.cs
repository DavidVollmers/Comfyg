using System.Text.Json.Serialization;
using Comfyg.Contracts.Configuration;

namespace Comfyg.Contracts.Responses;

public sealed class GetConfigurationValuesResponse : GetValuesResponse<IConfigurationValue>
{
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<IConfigurationValue>, IEnumerable<ConfigurationValue>,
            IEnumerable<IComfygValue>>))]
    public override IEnumerable<IConfigurationValue> Values { get; }

    public GetConfigurationValuesResponse(IEnumerable<IConfigurationValue> values)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
    }
}