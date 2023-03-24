using System.Text.Json.Serialization;
using Comfyg.Contracts.Settings;

namespace Comfyg.Contracts.Responses;

public class GetSettingValuesResponse : GetValuesResponse<ISettingValue>
{
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISettingValue>, IEnumerable<SettingValue>, IEnumerable<IComfygValue>>))]
    public override IEnumerable<ISettingValue> Values { get; }

    public GetSettingValuesResponse(IEnumerable<ISettingValue> values)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
    }
}
