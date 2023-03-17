using System.Text.Json.Serialization;
using Comfyg.Contracts.Settings;

namespace Comfyg.Contracts.Responses;

public class GetSettingValuesResponse
{
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISettingValue>, IEnumerable<SettingValue>, IEnumerable<IComfygValue>>))]
    public IEnumerable<ISettingValue> SettingValues { get; }

    public GetSettingValuesResponse(IEnumerable<ISettingValue> settingValues)
    {
        SettingValues = settingValues ?? throw new ArgumentNullException(nameof(settingValues));
    }
}